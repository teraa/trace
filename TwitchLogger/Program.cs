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

                    services
                        .AddDbContextFactory<TwitchLoggerDbContext>(options =>
                            options.UseNpgsql(Environment.GetEnvironmentVariable("DB_STRING")));

                    services
                        .AddSingleton<TwitchIrcClient>()
                        .AddHostedService<ChatClientService>()
                        .AddSingleton(chatClientConfig)
                        .AddHostedService<ChatLoggingService>()
                        .AddSingleton(chatLoggingConfig);
                });
    }
}
