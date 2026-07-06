using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SummerBot;
using SummerBot.Commands;
using SummerBot.Database.Data;
using Microsoft.EntityFrameworkCore;
using SummerBot.Events.PhotoContest;
using SummerBot.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration.GetSection("Discord");

        services.AddDiscordClient(config["Token"], intents: DiscordIntents.AllUnprivileged);

        services.AddCommandsExtension((_, commands) =>
        {
            commands.AddCommands(typeof(GeneralCommands).Assembly);
        });

        services.AddInteractivityExtension(new InteractivityConfiguration
        {
            PollBehaviour = PollBehaviour.KeepEmojis,
            Timeout = TimeSpan.FromSeconds(300)
        });

        services.ConfigureEventHandlers(b =>
        {
            b.AddEventHandlers<PhotoVoteHandler>(ServiceLifetime.Singleton);
            b.AddEventHandlers<PhotoDeleteHandler>(ServiceLifetime.Singleton);
        });

        services.AddHostedService<DiscordBotService>();

        var connString = context.Configuration.GetSection("Database")["ConnectionString"];
        services.AddDbContext<SummerBotDbContext>(options => options.UseSqlite(connString));

        services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            return configuration.GetSection("OpenWeatherMap")["ApiKey"] ?? "";
        });

        services.AddHttpClient<WeatherService>();
    })
    .Build();

await host.RunAsync();