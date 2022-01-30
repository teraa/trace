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
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContextFactory<TwitchLoggerDbContext>(options =>
                options.UseNpgsql(hostContext.Configuration["DB_STRING"]))

            .AddOptionsWithSection<ChatOptions>(hostContext.Configuration)
            .AddTwitchIrcClient()
            .AddHostedService<ChatService>()

            .AddOptionsWithSection<PubSubOptions>(hostContext.Configuration)
            .AddTwitchPubSubClient()
            .AddHostedService<PubSubService>()
            ;
    })
    .Build();

var contextFactory = host.Services.GetRequiredService<IDbContextFactory<TwitchLoggerDbContext>>();
using (var ctx = contextFactory.CreateDbContext())
{
    await ctx.Database.MigrateAsync();
}

await host.RunAsync();
