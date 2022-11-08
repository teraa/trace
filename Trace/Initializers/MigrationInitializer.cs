using Extensions.Hosting.AsyncInitialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Trace.Data;

namespace Trace.Initializers;

[UsedImplicitly]
public class MigrationInitializer : IAsyncInitializer
{
    private readonly TraceDbContext _ctx;

    public MigrationInitializer(TraceDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task InitializeAsync()
    {
        await _ctx.Database.MigrateAsync();
    }
}
