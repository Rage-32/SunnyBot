using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SunnyBot.Database.Data;

namespace SunnyBot.Events.PhotoContest;

public class PhotoDeleteHandler(IServiceProvider services) : IEventHandler<MessageDeletedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient sender, MessageDeletedEventArgs args)
    {
        if (args.Guild is null || args.Message is null)
            return;
        
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SunnyBotDbContext>();
        
        var photo = await db.PhotoSubmissions.FirstOrDefaultAsync(photo => photo.MessageId == args.Message.Id && photo.GuildId == args.Guild.Id);

        if (photo is null)
            return;

        db.PhotoVotes.RemoveRange(await db.PhotoVotes.Where(r => r.PhotoSubmissionId == photo.Id && r.GuildId == args.Guild.Id).ToListAsync());
        db.PhotoSubmissions.Remove(photo);
        await db.SaveChangesAsync();
    }
}