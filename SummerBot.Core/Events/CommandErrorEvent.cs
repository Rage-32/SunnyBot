using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.Commands.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SummerBot.Events;

public static class CommandErrorEvent
{
    public static async Task CommandsOnCommandErrored(CommandsExtension sender, CommandErroredEventArgs args)
    {
        if (args.Context is null)
            return;

        DiscordEmbedBuilder embed;

        switch (args.Exception)
        {
            case ArgumentParseException ape:
                embed = new DiscordEmbedBuilder()
                    .WithTitle("❌ Invalid Argument")
                    .WithDescription($"Couldn't parse `{ape.Parameter?.Name ?? "unknown"}`. Check the value and try again.")
                    .WithColor(0xFF6B35);
                break;
            
            case ChecksFailedException cfe when
                cfe.Errors.Count > 0 &&
                cfe.Errors[0].ContextCheckAttribute is RequirePermissionsAttribute rpa &&
                args.Context.Guild is not null:
            {
                if (args.Context.Member is not null && !args.Context.Member.Permissions.HasAllPermissions(rpa.UserPermissions))
                {
                    embed = new DiscordEmbedBuilder()
                        .WithTitle("❌ Missing Permission")
                        .WithDescription($"You need the following permissions to run this command: `{rpa.UserPermissions.ToString("name")}`")
                        .WithColor(0xFF6B35);
                }
                else
                {
                    embed = new DiscordEmbedBuilder()
                        .WithTitle("❌ Bot Missing Permission")
                        .WithDescription($"SummerBot needs the following permissions to run this command: `{rpa.BotPermissions.ToString("name")}`")
                        .WithColor(0xFF6B35);
                }
                break;
            }
            
            case ChecksFailedException cfe:
                var messages = new List<string>();
                
                foreach (var check in cfe.Errors)
                {
                    if (messages.Contains(check.ErrorMessage)) continue;
                    messages.Add($"{check.ErrorMessage}");
                }
                
                embed = new DiscordEmbedBuilder()
                    .WithTitle("❌ Failed")
                    .WithDescription($"{string.Join("\n", messages)}")
                    .WithColor(0xFF6B35);
                break;
            
            case UnauthorizedException:
                embed = new DiscordEmbedBuilder()
                    .WithTitle("❌ Unauthorized")
                    .WithDescription($"SummerBot is unauthorized to use command `{args.Context.Command.FullName}`.")
                    .WithColor(0xFF6B35);
                break;
            
            default:
                var logger = args.Context.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(args.Exception, "Command '{Command}' failed", args.Context.Command?.Name ?? "?");
                embed = new DiscordEmbedBuilder()
                    .WithTitle("❌ Something went wrong")
                    .WithDescription($"An unknown error occurred. Try again later or reach out for support. `{args.Exception.Message}`")
                    .WithColor(0xFF0000);
                break;
        }

        embed.WithFooter($"Command: /{args.Context.Command?.Name ?? "?"}");
        await args.Context.RespondAsync(embed);
    }
}