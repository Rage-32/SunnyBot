using DSharpPlus.Commands;
using Microsoft.EntityFrameworkCore;
using SummerBot.Database.Data;
using SummerBot.Database.Entities;

namespace SummerBot.Commands;

public class BucketListCommands(SummerBotDbContext db)
{
    [Command("bucket_add")]
    public async Task AddCommand(CommandContext ctx, string item)
    {
        var entry = new BucketListItem
        {
            UserId = ctx.User.Id,
            Description = item,
            CreatedAt = DateTime.UtcNow
        };
        
        db.BucketListItems.Add(entry);
        await db.SaveChangesAsync();
        
        await ctx.RespondAsync($"Added {item} to your bucket list!\n-# ID: {entry.Id}");
    }
    
    [Command("bucket_list")]
    public async Task ViewCommand(CommandContext ctx)
    {
        var items = await db.BucketListItems
            .Where(x => x.UserId == ctx.User.Id)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        if (items.Count == 0)
        {
            await ctx.RespondAsync("Your bucket list is empty! Add something with **bucket_add**.");
            return;
        }

        var total = items.Count;
        var done = items.Count(x => x.IsCompleted);
        var lines = items.Select((x, i) =>
            $"{(x.IsCompleted ? "~~" : "")}{i + 1}. {(x.IsCompleted ? "☑ " : "☐ ")} {x.Description}{(x.IsCompleted ? "~~" : "")}");
        
        var message = $"**Summer Bucket List ({done}/{total} complete)**\n{string.Join("\n", lines)}";
        await ctx.RespondAsync(message);
    }

    [Command("bucket_done")]
    public async Task DoneCommand(CommandContext ctx, int id)
    {
        var item = await db.BucketListItems.FindAsync(id);
        if (item is null || item.UserId != ctx.User.Id)
        {
            await ctx.RespondAsync("not found");
            return;
        }

        item.IsCompleted = true;
        item.CompletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        await ctx.RespondAsync($"✅ Completed \"{item.Description}\"!");
    }

    [Command("bucket_remove")]
    public async Task RemoveCommand(CommandContext ctx, int id)
    {
        var item = await db.BucketListItems.FindAsync(id);
        if (item is null || item.UserId != ctx.User.Id)
        {
            await ctx.RespondAsync("item not found");
            return;
        }

        db.BucketListItems.Remove(item);
        await db.SaveChangesAsync();

        await ctx.RespondAsync($"Removed \"{item.Description}\" from your bucket list.");
    }

    [Command("bucket_stats")]
    public async Task StatsCommand(CommandContext ctx)
    {
        var total = await db.BucketListItems.CountAsync(x => x.UserId == ctx.User.Id);
        var done = await db.BucketListItems.CountAsync(x => x.UserId == ctx.User.Id && x.IsCompleted);
        var pct = total > 0 ? (done * 100 / total) : 0;

        await ctx.RespondAsync($"**Your Bucket List Stats** - {done}/{total} completed ({pct}%)");
    }
}