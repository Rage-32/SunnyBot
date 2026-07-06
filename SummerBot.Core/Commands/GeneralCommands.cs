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

    [Command("weather")]
    public async Task WeatherCommand(CommandContext ctx, string city)
    {
        await ctx.DeferResponseAsync();

        var result = await weather.GetWeatherAsync(city);
        if (result is null)
        {
            var notFound = new DiscordEmbedBuilder()
                .WithTitle("❌ City Not Found")
                .WithDescription($"Couldn't find weather for \"{city}\".")
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