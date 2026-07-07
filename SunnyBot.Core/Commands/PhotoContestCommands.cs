using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using SunnyBot.Database.Data;
using SunnyBot.Database.Entities;
using SunnyBot.Helpers;

namespace SunnyBot.Commands;

[RequireGuild]
public class PhotoContestCommands(SunnyBotDbContext db)
{
    private readonly string[] _validFileFormats = [".png", ".jpeg", ".jpg"];
        
    [Command("photo_submit")]
    [Description("Submit a summer photo to the contest.")]
    public async Task SubmitCommand(CommandContext ctx,
        [Description("The image file to upload.")] DiscordAttachment attachment,
        [Description("A description for your photo.")] string? description = null)
    {
        await ctx.DeferResponseAsync();

        var ext = Path.GetExtension(attachment.FileName).ToLowerInvariant();
        if (!_validFileFormats.Contains(ext))
        {
            var err = new DiscordEmbedBuilder()
                .WithTitle("Invalid File")
                .WithDescription("Please attach a `.png`, `.jpeg`, or `.jpg` image.")
                .WithColor(new DiscordColor(0xFF0000));
            await ctx.RespondAsync(err);
            return;
        }

        var submission = new PhotoSubmission
        {
            GuildId = ctx.Guild!.Id,
            UserId = ctx.User.Id,
            ImageUrl = attachment.Url,
            Description = description ?? "",
            SubmittedAt = DateTime.UtcNow
        };

        db.PhotoSubmissions.Add(submission);
        await db.SaveChangesAsync();

        var embed = new DiscordEmbedBuilder()
            .WithTitle("📸 Summer Photo Submission")
            .WithDescription(string.IsNullOrEmpty(description) ? "No description provided." : description)
            .WithImageUrl(attachment.Url)
            .AddField("Submitted by", ctx.User.Mention)
            .AddField("👍 Upvotes", "0")
            .AddField("👎 Downvotes", "0")
            .WithColor(new DiscordColor(0xFF8C00))
            .WithFooter($"ID: {submission.Id}");

        var response = new DiscordInteractionResponseBuilder()
            .AddEmbed(embed)
            .AddActionRowComponent(new DiscordButtonComponent[]
            {
                new(DiscordButtonStyle.Success, $"photo_upvote_{submission.Id}", "👍"),
                new(DiscordButtonStyle.Danger, $"photo_downvote_{submission.Id}", "👎")
            });

        await ctx.RespondAsync(response);

        var messageResponse = await ctx.GetResponseAsync();
        if (messageResponse is null)
            return;

        submission.MessageId = messageResponse.Id;
        submission.MessageLink = messageResponse.JumpLink.ToString();
        await db.SaveChangesAsync();
    }

    [Command("photo_list")]
    [Description("Browse all submitted summer photos.")]
    public async Task ListCommand(CommandContext ctx)
    {
        var photos = await db.PhotoSubmissions
            .OrderByDescending(x => x.SubmittedAt)
            .ToListAsync();

        if (photos.Count == 0)
        {
            var empty = new DiscordEmbedBuilder()
                .WithTitle("📸 Summer Photos")
                .WithDescription("No photos submitted yet! Use `/photo_submit` to add one.")
                .WithColor(new DiscordColor(0xFF8C00));
            await ctx.RespondAsync(empty);
            return;
        }

        var index = 0;
        var pagination = await EmbedHelper.CreateEmbedPaginationAsync(
            user: ctx.User,
            items: photos,
            pageSize: 8,
            title: "📸 Recent Summer Photos",
            itemsLabel: "Submissions",
            color: new DiscordColor(0xFF8C00),
            descriptionBuilder: _ => "A list of all submitted Summer photos!",
            itemFormatter: submission =>
            {
                var messageLink = submission.MessageLink ?? "*No jump link found...*";
                return Task.FromResult($"{index + 1}. **ID {submission.Id}** - {messageLink} (👍 {submission.Upvotes} | 👎 {submission.Downvotes})");
            });

        await ctx.SendPaginatedMessageAsync(pagination);
    }

    [Command("photo_leaderboard")]
    [Description("View the top-ranked summer photos.")]
    public async Task LeaderboardCommand(CommandContext ctx)
    {
        var photos = await db.PhotoSubmissions
            .OrderByDescending(x => x.Upvotes - x.Downvotes)
            .ThenByDescending(x => x.SubmittedAt)
            .Take(10)
            .ToListAsync();

        if (photos.Count == 0)
        {
            var empty = new DiscordEmbedBuilder()
                .WithTitle("🏆 Photo Leaderboard")
                .WithDescription("No photos submitted yet! Use `/photo_submit` to add one.")
                .WithColor(new DiscordColor(0xFFD700));
            await ctx.RespondAsync(empty);
            return;
        }

        var lines = photos.Select((x, i) =>
        {
            var net = x.Upvotes - x.Downvotes;
            var medal = i switch { 0 => "🥇", 1 => "🥈", 2 => "🥉", _ => $"{i + 1}." };
            return $"{medal} {x.MessageLink ?? "*No jump link found...*"} - 👍 {x.Upvotes} 👎 {x.Downvotes} (net {net})";
        });

        var embed = new DiscordEmbedBuilder()
            .WithTitle("🏆 Photo Leaderboard")
            .WithDescription($"{string.Join("\n", lines)}")
            .WithColor(new DiscordColor(0xFFD700));

        await ctx.RespondAsync(embed);
    }

    [Command("photo_remove")]
    [Description("Remove a photo submission by ID. (Manage Guild)")]
    [RequirePermissions([], [DiscordPermission.ManageGuild])]
    public async Task RemoveCommand(CommandContext ctx,
        [Description("The ID of the photo to remove.")] int id)
    {
        var photo = await db.PhotoSubmissions.FirstOrDefaultAsync(a => a.Id == id && a.GuildId == ctx.Guild!.Id);
        
        if (photo is null)
        {
            await ctx.RespondAsync($"Failed to find photo by ID **{id}**.");
            return;
        }
        
        db.PhotoVotes.RemoveRange(await db.PhotoVotes.Where(vote => vote.PhotoSubmissionId == photo.Id).ToListAsync());
        db.PhotoSubmissions.Remove(photo);
        await db.SaveChangesAsync();

        await ctx.RespondAsync($"Successfully removed photo ID **{id}**!");
    }

    [Command("photo_reset")]
    [Description("Delete all photo submissions and votes. (Manage Guild)")]
    [RequirePermissions([], [DiscordPermission.ManageGuild])]
    public async Task ResetCommand(CommandContext ctx)
    {
        var photos = await db.PhotoSubmissions
            .Where(photo => photo.GuildId == ctx.Guild!.Id)
            .ToListAsync();

        var votes = await db.PhotoVotes
            .Where(vote => vote.GuildId == ctx.Guild!.Id)
            .ToListAsync();

        db.PhotoSubmissions.RemoveRange(photos);
        db.PhotoVotes.RemoveRange(votes);
        await db.SaveChangesAsync();

        await ctx.RespondAsync("Successfully reset Summer photos. All submissions have been deleted.");
    }
}
