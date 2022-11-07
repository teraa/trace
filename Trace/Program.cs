using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Teraa.Extensions.Configuration;
using Teraa.Extensions.Serilog.Seq;
using Teraa.Extensions.Serilog.Systemd;
using Teraa.Twitch.PubSub;
using Teraa.Twitch.Tmi;
using Trace.Data;
using Trace.Initializers;
using Trace.Options;
using Trace.Tmi;

var host = Host.CreateDefaultBuilder(args)
    .UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    })
    .UseSystemd()
    .UseSerilog((hostContext, options) =>
    {
        options
            .ReadFrom.Configuration(hostContext.Configuration)
            .ConfigureSystemdConsole()
            .ConfigureSeq(hostContext);
    })
    // ReSharper disable once UnusedParameter.Local
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
