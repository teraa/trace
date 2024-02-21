using Extensions.Hosting.AsyncInitialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Trace.Data;

namespace Trace.Migrations;

[UsedImplicitly]
public class MigrationInitializer : IAsyncInitializer
{
    private readonly TraceDbContext _ctx;

    public MigrationInitializer(TraceDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_ctx.Database.HasPendingModelChanges())
            throw new InvalidOperationException("Changes have been made to the model since the last migration. Add a new migration.");

        await _ctx.Database.MigrateAsync(cancellationToken);
    }
}
