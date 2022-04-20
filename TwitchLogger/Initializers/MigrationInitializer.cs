using Extensions.Hosting.AsyncInitialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using TwitchLogger.Data;

namespace TwitchLogger.Initializers;

[UsedImplicitly]
public class MigrationInitializer : IAsyncInitializer
{
    private readonly TwitchLoggerDbContext _ctx;

    public MigrationInitializer(TwitchLoggerDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task InitializeAsync()
    {
        await _ctx.Database.MigrateAsync();
    }
}
