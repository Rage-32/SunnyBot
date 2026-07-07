using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace SunnyBot.Commands;

public class GeneralCommands
{
    [Command("ping")]
    [Description("Test the connection latency.")]
    public async Task PingCommand(CommandContext ctx)
    {
        var embed = new DiscordEmbedBuilder()
            .WithTitle("🏓 Pong!")
            .WithDescription($"**Latency:** {ctx.Client.GetConnectionLatency(ctx.Guild?.Id ?? 0)}ms")
            .WithColor(new DiscordColor(0xFF8C00));
        
        await ctx.RespondAsync(embed);
    }

    [Command("invite")]
    [Description("Get the invite link for Sunny.")]
    public async Task InviteCommand(CommandContext ctx) =>
        await ctx.RespondAsync(new DiscordEmbedBuilder().WithColor(0xFFCC00).WithDescription("🌻 https://discord.com/oauth2/authorize?client_id=1523767352188080218&permissions=8&integration_type=0&scope=bot+applications.commands"));
    
    [Command("help")]
    [Description("View a list of all commands.")]
    public async Task HelpCommand(CommandContext ctx)
    {
        await ctx.DeferResponseAsync();
        
        var commands = await ctx.Client.GetGlobalApplicationCommandsAsync();

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Sunny Commands Help")
            .WithColor(new DiscordColor(0x5865F2))
            .WithDescription($"A list of all **{commands.Count}** commands.");

        embed.AddField("Commands", $"{string.Join(", ", commands.Select(r => r.Mention))}");

        await ctx.RespondAsync(embed);
    }
}