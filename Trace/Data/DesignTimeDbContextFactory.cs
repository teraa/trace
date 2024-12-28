using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Teraa.Shared.Configuration;

namespace Trace.Data;

[UsedImplicitly]
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>(optional: false)
            .Build();

        var dbOptions = config.GetValidatedRequiredOptions<DbOptions>();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(dbOptions.ConnectionString,
                contextOptions =>
                {
                    contextOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    contextOptions.CommandTimeout(600);
                });

        return new AppDbContext(optionsBuilder.Options);
    }
}
