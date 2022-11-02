using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Teraa.Extensions.Configuration;
using Teraa.Twitch.PubSub;
using Teraa.Twitch.Tmi;
using Trace;
using Trace.Data;
using Trace.Initializers;
using Trace.Options;
using Trace.Tmi;

var host = Host.CreateDefaultBuilder(args)
    .UseSystemd()
    .UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    })
    .ConfigureLogging((hostContext, builder) =>
    {
        builder.AddSeq(hostContext.Configuration.GetSection("Seq"));
    })
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddValidatorsFromAssemblyContaining<Program>()
            .AddAsyncInitializer<MigrationInitializer>()
            .AddOptionsWithValidation<DbOptions>()
            .AddDbContext<TraceDbContext>((sp, options) =>
            {
                using var scope = sp.CreateScope();
                var dbOptions = scope.ServiceProvider
                    .GetRequiredService<IOptionsMonitor<DbOptions>>()
                    .CurrentValue;

                options.UseNpgsql(dbOptions.ConnectionString, contextOptions =>
                {
                    contextOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    contextOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

#if DEBUG
                options.EnableSensitiveDataLogging();
#endif
            })
            .AddMemoryCache()
            .AddMediatR(typeof(Program))
            .AddSingleton<SourceCache>()
            .AddOptionsWithValidation<TmiOptions>()
            .AddTmiService()
            .AddOptionsWithValidation<PubSubOptions>()
            .AddPubSubService()
            ;
    })
    .Build();

await host.InitAsync();
await host.RunAsync();
