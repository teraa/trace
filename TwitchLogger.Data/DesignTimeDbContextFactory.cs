using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TwitchLogger.Data;

internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TwitchLoggerDbContext>
{
    public TwitchLoggerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TwitchLoggerDbContext>()
            .UseNpgsql(Environment.GetEnvironmentVariable("Db__ConnectionString")!);

        return new TwitchLoggerDbContext(optionsBuilder.Options);
    }
}
