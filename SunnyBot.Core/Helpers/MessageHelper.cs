using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace SunnyBot.Helpers;

public static class MessageHelper
{
    extension(CommandContext ctx)
    {
        public async Task SendPaginatedMessageAsync(List<Page> pages)
        {
            if (ctx is SlashCommandContext sctx)
            {
                await sctx.Interaction.SendPaginatedResponseAsync(false, ctx.User, pages);
                return;
            }
    
            await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages);
        }
    }
}