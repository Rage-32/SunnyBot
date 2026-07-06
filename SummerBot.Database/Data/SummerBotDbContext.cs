using Microsoft.EntityFrameworkCore;
using SummerBot.Database.Entities;

namespace SummerBot.Database.Data;

public class SummerBotDbContext(DbContextOptions<SummerBotDbContext> options) : DbContext(options)
{
    public DbSet<BucketListItem> BucketListItems => Set<BucketListItem>();
}