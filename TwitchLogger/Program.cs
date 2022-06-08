using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Teraa.Twitch.PubSub;
using Teraa.Twitch.Tmi;
using TwitchLogger;
using TwitchLogger.Data;
using TwitchLogger.Initializers;
using TwitchLogger.Tmi;

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
            .AddAsyncInitializer<MigrationInitializer>()
            .AddDbContext<TwitchLoggerDbContext>((sp, options) =>
            {
                var dbOptions = sp
                    .GetRequiredService<IConfiguration>()
                    .GetOptions<DbOptions>();

                options.UseNpgsql(dbOptions.ConnectionString, contextOptions =>
                {
                    contextOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    contextOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
            })

            .AddMediatR(typeof(Program))
            .AddSingleton<SourceCache>()
            .AddOptionsWithSection<TmiOptions>(hostContext.Configuration)
            .AddTmiService()
            .AddOptionsWithSection<PubSubOptions>(hostContext.Configuration)
            .AddPubSubService()

            .AddMemoryCache()
            .AddSingleton<MemoryCacheEntryOptions>(serviceProvider =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("CachePostEvictionCallback");

                var options = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                    .RegisterPostEvictionCallback((key, value, reason, _) =>
                    {
                        logger.LogDebug("Evicted: {Key}, {Value}, {Reason}", key, value, reason);
                    });

                return options;
            });
    })
    .Build();

await host.InitAsync();
await host.RunAsync();
