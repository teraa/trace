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
                .ConfigureServices((hostContext, services) =>
                {
                    var chatClientConfig = new ChatClientConfig
                    {
                        Channels = new [] { "tera_" }
                    };
                    var chatLoggingConfig = new ChatLoggingConfig
                    {
                        SourceId = 1
                    };
                    var pubSubClientConfig = new PubSubClientConfig
                    {
                        Token = Environment.GetEnvironmentVariable("PUBSUB_TOKEN")!,
                        Topics = new Topic[]
                        {
                            new Topic("chat_moderator_actions", new[] { "130277892", "52324616" })
                        }
                    };
                    var pubSubLoggingConfig = new PubSubLoggingConfig
                    {

                    };

                    services
                        .AddDbContextFactory<TwitchLoggerDbContext>(options =>
                            options.UseNpgsql(Environment.GetEnvironmentVariable("DB_STRING")));

                    services
                        .AddSingleton<TwitchIrcClient>()
                        .AddHostedService<ChatClientService>()
                        .AddSingleton(chatClientConfig)
                        .AddHostedService<ChatLoggingService>()
                        .AddSingleton(chatLoggingConfig);

                    services
                        .AddSingleton<TwitchPubSubClient>()
                        .AddHostedService<PubSubClientService>()
                        .AddSingleton(pubSubClientConfig)
                        .AddHostedService<PubSubLoggingService>()
                        .AddSingleton(pubSubLoggingConfig);
                });
    }
}
