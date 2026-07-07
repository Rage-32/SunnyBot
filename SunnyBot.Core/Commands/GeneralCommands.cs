using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace SunnyBot.Commands;

public class GeneralCommands
{
    private const string InviteUrl = "https://discord.com/oauth2/authorize?client_id=1523767352188080218&permissions=274878031872&integration_type=0&scope=bot+applications.commands";
    
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
        await ctx.RespondAsync(new DiscordEmbedBuilder().WithColor(0xFFCC00).WithDescription($"🌻 {InviteUrl}"));
    
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

    [Command("about")]
    [Description("Learn more about Sunny.")]
    public async Task AboutCommand(CommandContext ctx)
    {
        await ctx.DeferResponseAsync();
        
        var guilds = ctx.Client.Guilds.Values.ToList();
        var serverCount = guilds.Count;
        var userCount = guilds.Sum(g => g.MemberCount);
        
        var embed = new DiscordEmbedBuilder()
            .WithTitle("🌻 About Sunny")
            .WithColor(new DiscordColor(0xFF8C00))
            .WithDescription("Sunny is a summer-themed Discord bot - build a bucket list, share summer photos, check the weather, and more.")
            .AddField("📊 Stats", $"Servers: **{serverCount}**\nUsers: **{userCount:N0}**", true)
            .AddField("🔗 Links",
                $"[Invite Sunny]({InviteUrl})\n" +
                "[Support Server](https://discord.com/invite/aZEGSPKQMk)\n" +
                "[StellarBot](https://stellarbot.dev)", true)
            .AddField("👨‍💻 Developer", "rrage.", true)
            .WithFooter("☀️ Built for the Top.gg Summer Bot Jam 2026");

        await ctx.RespondAsync(embed);
    }
}