using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Twitch.Irc;
using Twitch.PubSub;
using TwitchLogger;
using TwitchLogger.Data;
using TwitchLogger.Options;
using TwitchLogger.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSystemd()
    .UseDefaultServiceProvider((hostContext, options) =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    })
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<TwitchLoggerDbContext>(contextOptions =>
            {
                contextOptions.UseNpgsql(
                    hostContext.Configuration["DB_STRING"],
                    npgsqlOpt => npgsqlOpt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            })

            .AddOptionsWithSection<ChatOptions>(hostContext.Configuration)
            .AddTwitchIrcClient()
            .AddHostedService<ChatService>()

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

using (var scope = host.Services.CreateScope())
{
    var ctx = scope.Get<TwitchLoggerDbContext>();
    await ctx.Database.MigrateAsync();
}

await host.RunAsync();
