using Extensions.Hosting.AsyncInitialization;

namespace Trace.Tests;

public class TestDbInitializer : IAsyncInitializer
{
    private readonly AppDbContext _ctx;

    public TestDbInitializer(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await _ctx.Database.EnsureDeletedAsync(cancellationToken);
        await _ctx.Database.EnsureCreatedAsync(cancellationToken);
    }
}
