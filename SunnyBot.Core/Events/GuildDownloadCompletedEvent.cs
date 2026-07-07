using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace SunnyBot.Events;

public class GuildDownloadCompletedEvent : IEventHandler<GuildDownloadCompletedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient sender, GuildDownloadCompletedEventArgs eventArgs)
    {
        await sender.UpdateStatusAsync(new DiscordActivity("🌞 Summer 2026"), DiscordUserStatus.Online);
    }
}