using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618
namespace TwitchLogger.Data;

[PublicAPI]
public partial class TwitchLoggerDbContext : DbContext
{
    public TwitchLoggerDbContext(DbContextOptions<TwitchLoggerDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TwitchLoggerDbContext).Assembly);
    }
}
