using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using SummerBot.Database.Data;
using SummerBot.Services;

namespace SummerBot.Commands;

public class SummerCommands(WeatherService weather, SummerBotDbContext db)
{
    [Command("summer_stats")]
    [Description("View summer stats for this server.")]
    [RequireGuild]
    public async Task StatsCommand(CommandContext ctx)
    {
        await ctx.DeferResponseAsync();
        
        var photoVotesCount = db.PhotoVotes.Count(pv => pv.GuildId == ctx.Guild!.Id);
        var photoCount = db.PhotoSubmissions.Count(ps => ps.GuildId == ctx.Guild!.Id);

        var bucketUniqueUsers = db.BucketListItems
            .Where(bl => bl.GuildId == ctx.Guild!.Id).Select(x => x.UserId)
            .Distinct()
            .Count();
        var bucketCount = db.BucketListItems.Count(bl => bl.GuildId == ctx.Guild!.Id);
        var bucketDoneCount = db.BucketListItems.Count(bl => bl.GuildId == ctx.Guild!.Id && bl.IsCompleted);

        var rate = bucketCount > 0 ? (int)Math.Round((double)bucketDoneCount / bucketCount * 100) : 0;
        
        var embed = new DiscordEmbedBuilder()
            .WithTitle("📊 SummerBot Server Stats")
            .WithColor(0xFF8C00)
            .AddField("Bucket List", $"Created: **{bucketCount}**\nCompleted: **{bucketDoneCount}**\nRate: **{rate}%**", true)
            .AddField("Photo Contest", $"Submissions: **{photoCount}**\nVotes: **{photoVotesCount}**", true)
            .AddField("Participants", $"**{bucketUniqueUsers}** member{(bucketUniqueUsers == 1 ? "" : "s")}", true);

        await ctx.RespondAsync(embed);
    }
    
    [Command("summer_fact")]
    [Description("Get a random summer-themed fact.")]
    public async Task SummerFactCommand(CommandContext ctx)
    {
        var fact = SummerFacts[Random.Shared.Next(SummerFacts.Length)];
        
        var embed = new DiscordEmbedBuilder()
            .WithTitle("☀️ Summer Fact")
            .WithDescription(fact)
            .WithColor(new DiscordColor(0xFF8C00));
        
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
    
    private static readonly string[] SummerFacts =
    [
        "The Eiffel Tower expands up to 15 cm in summer heat.",
        "Watermelon is 92% water - perfect for a hot day.",
        "The first ice cream cone was invented at the 1904 World's Fair.",
        "Summer solstice has the most daylight of any day of the year.",
        "Antarctica is actually farther from the sun in summer - the tilt matters more than distance.",
        "Ancient Greeks held the first Olympic Games in summer to honor Zeus.",
        "A single lightning bolt can heat the air to 30,000°C - five times hotter than the sun's surface.",
        "Sunflowers track the sun across the sky - a behavior called heliotropism.",
        "There's a beach in California made entirely of sea glass.",
        "The Great Barrier Reef can be seen from space - and it's at its most colorful in summer.",
        "The popsicle was invented by an 11-year-old in 1905, after he left a stirred drink outside overnight and it froze.",
        "The hottest temperature ever recorded on Earth was 56.7°C (134°F), in Death Valley in 1913.",
        "You can estimate the temperature by counting cricket chirps - it's called Dolbear's Law.",
        "Honeybees cool their hives in summer by fanning their wings in unison.",
        "Above the Arctic Circle, the sun doesn't set at all for part of summer - it's called the midnight sun.",
        "Watermelons are botanically related to cucumbers, both part of the gourd family.",
        "Sweat itself doesn't cool you down - it's the evaporation off your skin that does.",
        "The months July and August were renamed after Julius Caesar and Augustus, both Roman emperors.",
        "Fireflies use bioluminescent flashes to attract mates on summer nights, and each species has its own flash pattern.",
        "Ice cream sales spike on the hottest days, but historically, more is sold on Sundays than any other day of the week."
    ];
}