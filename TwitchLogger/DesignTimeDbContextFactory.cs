using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TwitchLogger.Data;

namespace TwitchLogger;

[UsedImplicitly]
internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TwitchLoggerDbContext>
{
    public TwitchLoggerDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>(optional: false)
            .Build();

        var dbOptions = config.GetOptions<DbOptions>();

        var optionsBuilder = new DbContextOptionsBuilder<TwitchLoggerDbContext>()
            .UseNpgsql(dbOptions.ConnectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                });

        return new TwitchLoggerDbContext(optionsBuilder.Options);
    }
}
