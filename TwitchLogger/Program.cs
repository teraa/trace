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

IHost host = Host.CreateDefaultBuilder(args)
    .UseSystemd()
    .UseDefaultServiceProvider((hostContext, options) =>
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
            .AddAsyncInitializer<MessageSourceInitializer>()
            .AddDbContext<TwitchLoggerDbContext>(contextOptions =>
            {
                contextOptions.UseNpgsql(
                    hostContext.Configuration["DB_STRING"],
                    npgsqlOpt => npgsqlOpt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            })

            .AddOptionsWithSection<ChatOptions>(hostContext.Configuration)
            .AddMediatR(typeof(Program))
            .AddTmiService()

            .AddOptionsWithSection<PubSubOptions>(hostContext.Configuration)
            .AddTwitchPubSubClient()
            .AddHostedService<PubSubService>()

            .AddMemoryCache()
            .AddSingleton<MemoryCacheEntryOptions>(services =>
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("CachePostEvictionCallback");

                var options = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                    .RegisterPostEvictionCallback((key, value, reason, state) =>
                    {
                        logger.LogDebug("Evicted ({key}, {value}): {reason}", key, value, reason);
                    });

                return options;
            });
    })
    .Build();

await host.InitAsync();
await host.RunAsync();
