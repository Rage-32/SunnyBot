namespace SummerBot.Database.Entities;

public class BucketListItem
{
    public int Id { get; set; }
    public ulong UserId { get; set; }
    public ulong GuildId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}