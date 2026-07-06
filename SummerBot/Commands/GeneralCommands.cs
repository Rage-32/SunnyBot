using System.ComponentModel;
using DSharpPlus.Commands;

namespace SummerBot.Commands;

public class GeneralCommands
{
    [Command("ping")]
    [Description("Test the connection latency.")]
    public async Task PingCommand(CommandContext ctx) =>
        await ctx.RespondAsync($"🏓 Pong! {ctx.Client.GetConnectionLatency(ctx.Guild?.Id ?? 0)}");
}