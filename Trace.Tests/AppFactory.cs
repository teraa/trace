using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Moq;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Teraa.Twitch.Tmi;
using Teraa.Twitch.Ws;
using Trace.Api;
using Trace.Api.Auth;
using Trace.Migrations;
using Trace.Tmi;

namespace Trace.Tests;

[Collection(AppFactoryFixture.CollectionName)]
public abstract class AppTests(AppFactory appFactory) : IAsyncLifetime
{
    protected AppFactory AppFactory { get; } = appFactory;

    public IServiceScope CreateScope() => AppFactory.Services.CreateScope();

    public Task InitializeAsync() => AppFactory.InitializeAsync();

    public Task DisposeAsync() => AppFactory.ResetDatabaseAsync();
}

[CollectionDefinition(CollectionName)]
public class AppFactoryFixture : ICollectionFixture<AppFactory>
{
    public const string CollectionName = "Default";
}

// ReSharper disable once ClassNeverInstantiated.Global
public class AppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly Regex s_allowedConnectionString =
        new(@"\bDatabase=\w+_tests\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private Respawner? _respawner;

    public Mock<IUserAccessor> UserAccessorMock { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveService<ITmiService>()
                .RemoveService<ITmiEventHandler>()
                .RemoveService<ITextWebSocketEventHandler>()
                ;

            services
                .RemoveAll<IValidator<TmiOptions>>()
                .RemoveAll<IValidator<TwitchAuthOptions>>();

            services.RemoveService<SourceInitializer>();

            services
                .RemoveAll<IUserAccessor>()
                .AddTransient<IUserAccessor>(_ => UserAccessorMock.Object);

            services
                .RemoveService<MigrationInitializer>()
                .AddAsyncInitializer<TestDbInitializer>();
        });
    }

    public async Task ResetDatabaseAsync(CancellationToken cancellationToken = default)
    {
        var options = Services.GetRequiredService<IOptions<DbOptions>>().Value;

        if (!s_allowedConnectionString.IsMatch(options.ConnectionString))
        {
            throw new InvalidOperationException(
                """Tests can only run on databases with a name ending with "_tests", please check your connection string."""
            );
        }

        await using var connection = new NpgsqlConnection(options.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        _respawner ??= await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToIgnore = [new Table("__EFMigrationsHistory")],
                SchemasToInclude = ["public", "twitch", "tmi", "pubsub"],
            }
        );

        await _respawner.ResetAsync(connection);
    }

    public Task InitializeAsync()
    {
        UserAccessorMock = new Mock<IUserAccessor>(MockBehavior.Strict);

        return Task.CompletedTask;
    }

    public new async Task DisposeAsync()
    {
        // Delete the database after all tests have completed.
        using var scope = Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await ctx.Database.EnsureDeletedAsync();
    }
}
