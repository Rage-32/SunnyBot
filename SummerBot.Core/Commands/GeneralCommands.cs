using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using SummerBot.Services;

namespace SummerBot.Commands;

public class GeneralCommands(WeatherService weather)
{
    [Command("ping")]
    [Description("Test the connection latency.")]
    public async Task PingCommand(CommandContext ctx) =>
        await ctx.RespondAsync($"🏓 Pong! {ctx.Client.GetConnectionLatency(ctx.Guild?.Id ?? 0)}");

    [Command("weather")]
    public async Task WeatherCommand(CommandContext ctx, string city)
    {
        await ctx.DeferResponseAsync();

        var result = await weather.GetWeatherAsync(city);
        if (result is null)
        {
            await ctx.RespondAsync("CIty not found.");
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