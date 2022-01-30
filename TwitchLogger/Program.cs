using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Twitch.Irc;
using Twitch.PubSub;
using TwitchLogger.Data;
using TwitchLogger.Options;
using TwitchLogger.Services;

namespace TwitchLogger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddDbContextFactory<TwitchLoggerDbContext>(options =>
                            options.UseNpgsql(Environment.GetEnvironmentVariable("DB_STRING")!))

                        .AddOptionsWithSection<PubSubOptions>(hostContext.Configuration)
                        .AddOptionsWithSection<ChatOptions>(hostContext.Configuration)
                        ;

                    services
                        .AddTwitchIrcClient()
                        .AddHostedService<ChatClientService>()
                        .AddHostedService<ChatLoggingService>();

                    services
                        .AddTwitchPubSubClient()
                        .AddHostedService<PubSubClientService>()
                        .AddHostedService<PubSubLoggingService>();
                });
    }
}
