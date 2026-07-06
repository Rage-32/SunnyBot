using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace SummerBot.Helpers;

public class EmbedHelper
{
    /// <summary>
    /// Create a paginated embed component
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Page>> CreateEmbedPaginationAsync<T>(
        DiscordUser user,
        List<T> items, int pageSize,
        string title,
        DiscordColor color, 
        Func<T, Task<string>> itemFormatter, 
        string itemsLabel = "Items", 
        Func<List<T>, string>? descriptionBuilder = null)
    {
        var pages = new List<Page>();
        var pageCount = (int)Math.Ceiling(items.Count / (double)pageSize);

        for (var i = 0; i < pageCount; i++)
        {
            var currentPage = i + 1;
            var start = i * pageSize;
            var remainingItems = items.Count - start;
            var itemsToDisplay = Math.Min(pageSize, remainingItems);

            var embed = new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithColor(color)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter($"Page {currentPage} of {pageCount}")
                .WithThumbnail(user.AvatarUrl);

            if (descriptionBuilder is not null)
                embed.WithDescription(descriptionBuilder(items));
            
            var itemsList = string.Join("\n", await Task.WhenAll(items
                .Skip(start)
                .Take(itemsToDisplay)
                .Select(itemFormatter)));
            
            embed.AddField($"{itemsLabel} [{start + itemsToDisplay}/{items.Count}]", itemsList);
            
            pages.Add(new Page { Embed = embed });
        }

        return pages;
    }
}