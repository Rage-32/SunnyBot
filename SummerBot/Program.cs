using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SummerBot;
using SummerBot.Commands;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration.GetSection("Discord");

        services.AddDiscordClient(config["Token"], intents: DiscordIntents.AllUnprivileged);

        services.AddCommandsExtension((_, commands) =>
        {
            commands.AddCommands<GeneralCommands>();
        });

        services.AddHostedService<DiscordBotService>();
    })
    .Build();

await host.RunAsync();