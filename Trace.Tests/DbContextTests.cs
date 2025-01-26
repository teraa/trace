using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

namespace Trace.Tests;

[Collection(AppFactoryFixture.CollectionName)]
public class DbContextTests
{
    private readonly WebApplicationFactory<Program> _appFactory;

    public DbContextTests(AppFactory appFactory)
    {
        _appFactory = appFactory.WithWebHostBuilder(
            builder => builder.ConfigureTestServices(
                services => services.RemoveService<TestDbInitializer>()
            )
        );
    }

    [Fact]
    public async Task MigrationsRunSuccessfully()
    {
        using var scope = _appFactory.Services.CreateScope();
        var ctx = scope.Get<AppDbContext>();

        await ctx.Database.EnsureDeletedAsync();
        await ctx.Database.MigrateAsync();
    }

    [Fact]
    public async Task ReverseMigrationsRunSuccessfully()
    {
        using var scope = _appFactory.Services.CreateScope();
        var ctx = scope.Get<AppDbContext>();

        var migrations = ctx.Database.GetMigrations().Reverse();

        foreach (var migration in migrations)
        {
            await ctx.Database.MigrateAsync(migration);
        }
    }
}
