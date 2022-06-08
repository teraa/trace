using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Teraa.Twitch.Tmi;
using Twitch.PubSub;
using TwitchLogger;
using TwitchLogger.Data;
using TwitchLogger.Initializers;
using TwitchLogger.Options;
using TwitchLogger.Services;

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
            .AddAsyncInitializer<TmiSourceInitializer>()
            .AddDbContext<TwitchLoggerDbContext>((sp, options) =>
            {
                var dbOptions = sp
                    .GetRequiredService<IConfiguration>()
                    .GetOptions<DbOptions>();

                options.UseNpgsql(dbOptions.ConnectionString, contextOptions =>
                {
                    contextOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
            })

            .AddOptionsWithSection<ChatOptions>(hostContext.Configuration)
            .AddMediatR(typeof(Program))
            .AddTmiService()

            .AddOptionsWithSection<PubSubOptions>(hostContext.Configuration)
            .AddTwitchPubSubClient()
            .AddHostedService<PubSubService>()

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
