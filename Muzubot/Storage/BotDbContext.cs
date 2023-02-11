using Microsoft.EntityFrameworkCore;
using Muzubot.Storage.Models;

namespace Muzubot.Storage;

public class BotDbContext : DbContext
{
    public BotDbContext(Configuration config)
    {
        _config = config;
    }

    public DbSet<DungeonData> Dungeon { get; set; }
    public DbSet<CommandUsageData> CommandUsage { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_config.DbConnectionString)
            .UseSnakeCaseNamingConvention();

    private readonly Configuration _config;
}