using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Twitch.Irc;
using Twitch.PubSub;
using TwitchLogger.Data;
using TwitchLogger.Options;
using TwitchLogger.Services;

namespace TwitchLogger;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var contextFactory = host.Services.GetRequiredService<IDbContextFactory<TwitchLoggerDbContext>>();
            using (var ctx = contextFactory.CreateDbContext())
            {
                await ctx.Database.MigrateAsync();
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddDbContextFactory<TwitchLoggerDbContext>(options =>
                            options.UseNpgsql(Environment.GetEnvironmentVariable("DB_STRING")!))

                        .AddOptionsWithSection<ChatOptions>(hostContext.Configuration)
                        .AddTwitchIrcClient()
                        .AddHostedService<ChatService>()

                        .AddOptionsWithSection<PubSubOptions>(hostContext.Configuration)
                        .AddTwitchPubSubClient()
                        .AddHostedService<PubSubService>()
                        ;
                });
    }
