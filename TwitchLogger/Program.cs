using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            ;
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var ctx = scope.Get<TwitchLoggerDbContext>();
    await ctx.Database.MigrateAsync();
}

await host.RunAsync();
