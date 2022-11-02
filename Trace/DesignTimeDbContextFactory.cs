using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Teraa.Extensions.Configuration;
using Trace.Data;
using Trace.Options;

namespace Trace;

[UsedImplicitly]
internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TraceDbContext>
{
    public TraceDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>(optional: false)
            .Build();

        var dbOptions = config.GetOptions<DbOptions>();

        var optionsBuilder = new DbContextOptionsBuilder<TraceDbContext>()
            .UseNpgsql(dbOptions.ConnectionString,
                contextOptions =>
                {
                    contextOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    contextOptions.CommandTimeout(600);
                });

        return new TraceDbContext(optionsBuilder.Options);
    }
}
