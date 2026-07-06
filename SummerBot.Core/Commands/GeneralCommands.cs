using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using SummerBot.Services;

namespace SummerBot.Commands;

public class GeneralCommands(WeatherService weather)
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

    [Command("Help")]
    [Description("View a list of all commands.")]
    public async Task HelpCommand(CommandContext ctx)
    {
        await ctx.DeferResponseAsync();
        
        var commands = await ctx.Client.GetGlobalApplicationCommandsAsync();

        var embed = new DiscordEmbedBuilder()
            .WithTitle("SummerBot Commands Help")
            .WithColor(new DiscordColor(0x5865F2))
            .WithDescription($"A list of all **{commands.Count}** commands.");

        embed.AddField("Commands", $"{string.Join(", ", commands.Select(r => r.Mention))}");

        await ctx.RespondAsync(embed);
    }

    [Command("weather")]
    [Description("View the weather in a location.")]
    public async Task WeatherCommand(CommandContext ctx, [Description("The location to view the weather in.")] string location)
    {
        await ctx.DeferResponseAsync();

        var result = await weather.GetWeatherAsync(location);
        if (result is null)
        {
            var notFound = new DiscordEmbedBuilder()
                .WithTitle("❌ City Not Found")
                .WithDescription($"Couldn't find weather for \"{location}\".")
                .WithColor(new DiscordColor(0xFF6B35));
            await ctx.RespondAsync(notFound);
            return;
        }

        var emoji = result.Main.Temp switch
        {
            > 35 => "🥵",
            > 25 => "☀️",
            > 15 => "🌤️",
            > 5 => "☁️",
            _ => "❄️"
        };
        
        var embed = new DiscordEmbedBuilder()
            .WithTitle($"Weather in {result.Name}, {result.Sys.Country}")
            .WithDescription($"{emoji} {result.Weather[0].Description}")
            .AddField("Temperature", $"{result.Main.Temp:F1}°C", true)
            .AddField("Feels Like", $"{result.Main.FeelsLike:F1}°C", true)
            .AddField("Humidity", $"{result.Main.Humidity}%", true)
            .AddField("Wind", $"{result.Wind.Speed:F1} m/s", true)
            .WithColor(new DiscordColor(0xFF8C00));

        await ctx.RespondAsync(embed);

    }
}