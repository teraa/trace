using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Twitch.Irc;
using Twitch.PubSub;
using TwitchLogger.Data;
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
                    var chatClientConfig = new ChatClientConfig
                    {

                    };
                    var chatLoggingConfig = new ChatLoggingConfig
                    {
                        SourceName = Environment.GetEnvironmentVariable("CHAT_MESSAGE_SOURCE")!
                    };
                    var pubSubClientConfig = new PubSubClientConfig
                    {
                        Token = Environment.GetEnvironmentVariable("PUBSUB_TOKEN")!
                    };
                    var pubSubLoggingConfig = new PubSubLoggingConfig
                    {

                    };

                    services
                        .AddDbContextFactory<TwitchLoggerDbContext>(options =>
                            options.UseNpgsql(Environment.GetEnvironmentVariable("DB_STRING")));

                    services
                        .AddTwitchIrcClient()
                        .AddHostedService<ChatClientService>()
                        .AddSingleton(chatClientConfig)
                        .AddHostedService<ChatLoggingService>()
                        .AddSingleton(chatLoggingConfig);

                    services
                        .AddTwitchPubSubClient()
                        .AddHostedService<PubSubClientService>()
                        .AddSingleton(pubSubClientConfig)
                        .AddHostedService<PubSubLoggingService>()
                        .AddSingleton(pubSubLoggingConfig);
                });
    }
}
