namespace SunnyBot.Database.Entities;

public class PhotoVote
{
    public int Id { get; set; }
    public int PhotoSubmissionId { get; set; }
    public ulong GuildId { get; set; }
    public ulong UserId { get; set; }
    public bool IsUpVote { get; set; }
}