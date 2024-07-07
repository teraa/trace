using System.Security.Claims;
using System.Text.RegularExpressions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Teraa.Twitch.PubSub;
using Teraa.Twitch.Tmi;
using Trace.Api;
using Trace.Data;
using Trace.PubSub;
using Trace.Tmi;

namespace Trace.Tests;

[CollectionDefinition("1")]
public class AppFactoryFixture : ICollectionFixture<AppFactory>;

// ReSharper disable once ClassNeverInstantiated.Global
public class AppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly Regex s_allowedConnectionString =
        new(@"\bDatabase=\w+_test\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly UserAccessor _userAccessor = new();
    private Respawner? _respawner;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveService<TmiService>()
                .RemoveService<PubSubService>()
                .RemoveAll(x =>
                    x.ServiceType.IsGenericType &&
                    x.ServiceType.GetGenericTypeDefinition() == typeof(INotificationHandler<>));

            services
                .RemoveAll<IValidator<TmiOptions>>()
                .RemoveAll<IValidator<PubSubOptions>>()
                .RemoveAll<IValidator<TwitchAuthOptions>>();

            services.RemoveService<SourceInitializer>();

            services
                .RemoveAll<IUserAccessor>()
                .AddSingleton<IUserAccessor>(_userAccessor);
        });
    }

    public async Task ResetDatabaseAsync(CancellationToken cancellationToken = default)
    {
        var options = Services.GetRequiredService<IOptions<DbOptions>>().Value;

        if (!s_allowedConnectionString.IsMatch(options.ConnectionString))
        {
            throw new InvalidOperationException(
                """Tests can only run on databases with a name ending with "_test", please check your appsettings file."""
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

    public void SetUser(ClaimsPrincipal user) => _userAccessor.User = user;

    public void SetUser(IEnumerable<Claim> claims) =>
        _userAccessor.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public new async Task DisposeAsync()
    {
        // In case something else forgot to reset DB, this will clean after all tests are done.
        await ResetDatabaseAsync();
    }
}

public class UserAccessor : IUserAccessor
{
    private ClaimsPrincipal? _user;

    public ClaimsPrincipal User
    {
        get => _user ?? throw new InvalidOperationException("User not set");
        set => _user = value;
    }
}

file static class Extensions
{
    public static IServiceCollection RemoveAll(
        this IServiceCollection services,
        Func<ServiceDescriptor, bool> predicate)
    {
        var descriptors = services.Where(predicate).ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }

        return services;
    }

    public static IServiceCollection RemoveService<TService>(this IServiceCollection services)
    {
        services.RemoveAll(x =>
            x.ServiceType == typeof(TService) ||
            x.ImplementationType == typeof(TService) ||
            (x.ImplementationFactory is not null &&
             x.ImplementationFactory.Method.ReturnType == typeof(TService)));

        return services;
    }
}
