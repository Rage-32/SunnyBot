using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummerBot.Database.Data;
using SummerBot.Database.Entities;

namespace SummerBot.Events.PhotoContest;

public class PhotoVoteHandler(IServiceProvider services) : IEventHandler<ComponentInteractionCreatedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args)
    {
        if (args.Guild is null)
            return;
        
        var customId = args.Interaction.Data.CustomId;
        if (!customId.StartsWith("photo_"))
            return;
        
        var parts = customId.Split('_');
        if (parts.Length < 3 || !int.TryParse(parts[2], out var photoId))
            return;
        
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SummerBotDbContext>();
        var photo = await db.PhotoSubmissions.FirstOrDefaultAsync(sb => sb.Id == photoId && sb.GuildId == args.Guild.Id);

        if (photo is null)
        {
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("Photo not found.")
                    .AsEphemeral());
            return;
        }
        
        var alreadyVoted = await db.PhotoVotes.AnyAsync(v => v.PhotoSubmissionId == photo.Id && v.UserId == args.User.Id);
        if (alreadyVoted)
        {
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("You already voted.")
                    .AsEphemeral());
            return;
        }

        switch (parts[1])
        {
            case "upvote":
                photo.Upvotes++;
                break;
            case "downvote":
                photo.Downvotes++;
                break;
            default:
                return;
        }

        db.PhotoVotes.Add(new PhotoVote
        {
            GuildId = args.Guild.Id,
            PhotoSubmissionId = photo.Id,
            UserId = args.User.Id,
            IsUpVote = parts[1] == "upvote"
        });

        await db.SaveChangesAsync();
        
        var embed = new DiscordEmbedBuilder()
            .WithTitle("📸 Photo Submission")
            .WithImageUrl(photo.ImageUrl)
            .WithDescription(photo.Description)
            .AddField("Submitted by", $"<@{photo.UserId}>")
            .AddField("👍 Upvotes", photo.Upvotes.ToString())
            .AddField("👎 Downvotes", photo.Downvotes.ToString())
            .WithColor(new DiscordColor(0xFF8C00))
            .WithFooter($"ID: {photo.Id}");
        
        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
            new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .AddActionRowComponent(new DiscordButtonComponent[]
                {
                    new(DiscordButtonStyle.Success, $"photo_upvote_{photo.Id}", "👍"),
                    new(DiscordButtonStyle.Danger, $"photo_downvote_{photo.Id}", "👎")
                }));
    }
}
