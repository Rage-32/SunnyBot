using Microsoft.EntityFrameworkCore;
using SummerBot.Database.Entities;

namespace SummerBot.Database.Data;

public class SummerBotDbContext(DbContextOptions<SummerBotDbContext> options) : DbContext(options)
{
    public DbSet<BucketListItem> BucketListItems => Set<BucketListItem>();
    public DbSet<PhotoSubmission> PhotoSubmissions => Set<PhotoSubmission>();
    public DbSet<PhotoVote> PhotoVotes => Set<PhotoVote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PhotoVote>()
            .HasIndex(v => new { v.PhotoSubmissionId, v.UserId })
            .IsUnique();
    }
}