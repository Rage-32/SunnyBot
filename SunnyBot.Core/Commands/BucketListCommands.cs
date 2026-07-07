using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using SummerBot.Database.Data;
using SummerBot.Database.Entities;

namespace SummerBot.Commands;

public class BucketListCommands(SummerBotDbContext db)
{
    [Command("bucket_add")]
    [Description("Add an item to your summer bucket list.")]
    public async Task AddCommand(CommandContext ctx, [Description("The item to add.")] string item)
    {
        var entry = new BucketListItem
        {
            UserId = ctx.User.Id,
            GuildId = ctx.Guild?.Id ?? 0,
            Description = item,
            CreatedAt = DateTime.UtcNow
        };
        
        db.BucketListItems.Add(entry);
        await db.SaveChangesAsync();
        
        var count = await db.BucketListItems.CountAsync(x => x.UserId == ctx.User.Id);
        var embed = new DiscordEmbedBuilder()
            .WithTitle("☀️ Added to Bucket List")
            .WithDescription(item)
            .AddField("ID", entry.Id.ToString(), true)
            .AddField("Total Items", count.ToString(), true)
            .WithColor(new DiscordColor(0x00B4D8));
        await ctx.RespondAsync(embed);
    }
    
    [Command("bucket_list")]
    [Description("View all items in your summer bucket list.")]
    public async Task ViewCommand(CommandContext ctx)
    {
        var items = await db.BucketListItems
            .Where(x => x.UserId == ctx.User.Id)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        if (items.Count == 0)
        {
            var empty = new DiscordEmbedBuilder()
                .WithTitle("📋 Summer Bucket List")
                .WithDescription("Your bucket list is empty! Use `/bucket_add` to add something.")
                .WithColor(new DiscordColor(0x00B4D8));
            await ctx.RespondAsync(empty);
            return;
        }

        var total = items.Count;
        var done = items.Count(x => x.IsCompleted);
        var pct = done * 100 / total;
        var bar = new string('█', done * 10 / total) + new string('░', 10 - done * 10 / total);
        var lines = items.Select((x, i) =>
            $"{(x.IsCompleted ? "~~" : "")}{i + 1}. {(x.IsCompleted ? "☑ " : "☐ ")} {x.Description}{(x.IsCompleted ? "~~" : "")}");
        
        var list = new DiscordEmbedBuilder()
            .WithTitle($"📋 Summer Bucket List ({done}/{total})")
            .WithDescription(string.Join("\n", lines))
            .WithFooter($"{bar} {pct}%")
            .WithColor(new DiscordColor(0x00B4D8));
        await ctx.RespondAsync(list);
    }

    [Command("bucket_done")]
    [Description("Mark a bucket list item as completed.")]
    public async Task DoneCommand(CommandContext ctx, [Description("The ID of the item to mark complete.")] int id)
    {
        var item = await db.BucketListItems.FindAsync(id);
        if (item is null || item.UserId != ctx.User.Id)
        {
            var notFound = new DiscordEmbedBuilder()
                .WithTitle("❌ Not Found")
                .WithDescription($"No item with ID {id} in your bucket list.")
                .WithColor(new DiscordColor(0xFF6B35));
            await ctx.RespondAsync(notFound);
            return;
        }

        item.IsCompleted = true;
        item.CompletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var doneEmbed = new DiscordEmbedBuilder()
            .WithTitle("✅ Completed!")
            .WithDescription(item.Description)
            .WithColor(new DiscordColor(0x00B4D8));
        await ctx.RespondAsync(doneEmbed);
    }

    [Command("bucket_remove")]
    [Description("Remove an item from your bucket list.")]
    public async Task RemoveCommand(CommandContext ctx, [Description("The ID of the item to remove.")] int id)
    {
        var item = await db.BucketListItems.FindAsync(id);
        if (item is null || item.UserId != ctx.User.Id)
        {
            var notFound = new DiscordEmbedBuilder()
                .WithTitle("❌ Not Found")
                .WithDescription($"No item with ID {id} in your bucket list.")
                .WithColor(new DiscordColor(0xFF6B35));
            await ctx.RespondAsync(notFound);
            return;
        }

        db.BucketListItems.Remove(item);
        await db.SaveChangesAsync();

        var removed = new DiscordEmbedBuilder()
            .WithTitle("🗑️ Removed")
            .WithDescription(item.Description)
            .WithColor(new DiscordColor(0x00B4D8));
        await ctx.RespondAsync(removed);
    }

    [Command("bucket_stats")]
    [Description("View your bucket list completion stats.")]
    public async Task StatsCommand(CommandContext ctx)
    {
        var total = await db.BucketListItems.CountAsync(x => x.UserId == ctx.User.Id);
        var done = await db.BucketListItems.CountAsync(x => x.UserId == ctx.User.Id && x.IsCompleted);
        var pct = total > 0 ? (done * 100 / total) : 0;
        var bar = total > 0 ? new string('█', done * 10 / total) + new string('░', 10 - done * 10 / total) : "░░░░░░░░░░";

        var stats = new DiscordEmbedBuilder()
            .WithTitle("📊 Bucket List Stats")
            .WithDescription($"**{done}/{total}** completed ({pct}%)")
            .WithFooter($"{bar} {pct}%")
            .WithColor(new DiscordColor(0xFFD700));
        await ctx.RespondAsync(stats);
    }
}