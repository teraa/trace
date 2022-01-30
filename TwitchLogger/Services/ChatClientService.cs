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
    class ChatClientService : IHostedService
    {
        private readonly TwitchIrcClient _client;
        private readonly ILogger<ChatClientService> _logger;
        private readonly IDbContextFactory<TwitchLoggerDbContext> _contextFactory;


        public ChatClientService(
            TwitchIrcClient client,
            ILogger<ChatClientService> logger,
            IDbContextFactory<TwitchLoggerDbContext> contextFactory)
        {
            _client = client;
            _logger = logger;

            _client.Connected += ConnectedAsync;
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

        private async ValueTask ConnectedAsync()
        {
            await _client.LoginAnonAsync();

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
                tasks.Add(_client.SendAsync(new IrcMessage { Command = IrcCommand.JOIN, Content = new($"#{channelLogin}") }).AsTask());

            await Task.WhenAll(tasks);

            _logger.LogInformation($"Joined {channelLogins.Length} channels: {string.Join(", ", channelLogins)}");
        }
    }
}
