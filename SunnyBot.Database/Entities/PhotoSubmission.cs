namespace SunnyBot.Database.Entities;

public class PhotoSubmission
{
    public int Id { get; set; }
    public ulong UserId { get; set; }
    public ulong GuildId { get; set; }
    public ulong MessageId { get; set; }
    public string? MessageLink { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
}
