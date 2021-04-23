using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IrcMessageParser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Twitch.Irc;
using TwitchLogger.Data;

namespace TwitchLogger.Services
{
    class ChatClientConfig
    {

    }

    class ChatClientService : IHostedService
    {
        private readonly TwitchIrcClient _client;
        private readonly ILogger<ChatClientService> _logger;
        private readonly ChatClientConfig _config;
        private readonly IDbContextFactory<TwitchLoggerDbContext> _contextFactory;


        public ChatClientService(
            TwitchIrcClient client,
            ILogger<ChatClientService> logger,
            ChatClientConfig config,
            IDbContextFactory<TwitchLoggerDbContext> contextFactory)
        {
            _client = client;
            _logger = logger;
            _config = config;

            _client.Ready += ReadyAsync;
            _contextFactory = contextFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.ConnectAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();
        }

        private Task ReadyAsync()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    string[] channelLogins;

                    using (var ctx = _contextFactory.CreateDbContext())
                    {
                        channelLogins = await ctx.ChatLogs
                            .AsQueryable()
                            .Select(x => x.Channel.Login)
                            .ToArrayAsync();
                    }

                    var tasks = new List<Task>();
                    foreach (var channelLogin in channelLogins)
                        tasks.Add(_client.SendAsync(new IrcMessage { Command = IrcCommand.JOIN, Content = new($"#{channelLogin}") }));

                    await Task.WhenAll(tasks);

                    _logger.LogInformation($"Joined {channelLogins.Length} channels: {string.Join(", ", channelLogins)}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception while joining channels.");
                }
            });

            return Task.CompletedTask;
        }
    }
}
