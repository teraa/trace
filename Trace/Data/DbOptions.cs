﻿using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Teraa.Extensions.Configuration;
using Trace.Migrations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Trace.Data;

#pragma warning disable CS8618
public class DbOptions
{
    public string ConnectionString { get; init; }

    [UsedImplicitly]
    public class Validator : AbstractValidator<DbOptions>
    {
        public Validator()
        {
            RuleFor(x => x.ConnectionString).NotEmpty();
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDb(this IServiceCollection services)
    {
        services
            .AddAsyncInitializer<MigrationInitializer>()
            .AddValidatedOptions<DbOptions>()
            .AddDbContext<AppDbContext>(static (services, options) =>
            {
                using var scope = services.CreateScope();
                var dbOptions = scope.ServiceProvider
                    .GetRequiredService<IOptionsMonitor<DbOptions>>()
                    .CurrentValue;

                options.UseNpgsql(dbOptions.ConnectionString, contextOptions =>
                {
                    contextOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

#if DEBUG
                options.EnableSensitiveDataLogging();
#endif
            });

        return services;
    }
}
