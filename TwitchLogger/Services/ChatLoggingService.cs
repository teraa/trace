using System;
using System.Threading;
using System.Threading.Tasks;
using IrcMessageParser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Twitch.Irc;
using TwitchLogger.Data;
using TwitchLogger.Data.Models;

namespace TwitchLogger.Services
{
    class ChatLoggingConfig
    {
        public short SourceId { get; set; }
    }

    class ChatLoggingService : IHostedService
    {
        private readonly TwitchIrcClient _client;
        private readonly ILogger<ChatLoggingService> _logger;
        private readonly IDbContextFactory<TwitchLoggerDbContext> _contextFactory;
        private readonly ChatLoggingConfig _config;

        public ChatLoggingService(
            TwitchIrcClient client,
            ILogger<ChatLoggingService> logger,
            IDbContextFactory<TwitchLoggerDbContext> contextFactory,
            ChatLoggingConfig config)
        {
            _client = client;
            _logger = logger;
            _contextFactory = contextFactory;
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.IrcMessageReceived += IrcMessageReceivedAsync;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.IrcMessageReceived -= IrcMessageReceivedAsync;

            return Task.CompletedTask;
        }

        private async Task IrcMessageReceivedAsync(IrcMessage message)
        {
            if (message.Command != IrcCommand.PRIVMSG) return;

            var channelLogin = message.Arg![1..];
            var channelId =  message.Tags!["room-id"];
            var userLogin = message.Hostmask![..message.Hostmask.IndexOf('!')];
            var userId = message.Tags!["user-id"];
            var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(message.Tags["tmi-sent-ts"]));

            using var ctx = _contextFactory.CreateDbContext();

            await ctx.CreateOrUpdateUserAsync(new User { Id = channelId, Login = channelLogin, FirstSeenAt = timestamp });
            await ctx.CreateOrUpdateUserAsync(new User { Id = userId, Login = userLogin, FirstSeenAt = timestamp });

            var messageEntity = new Message
            {
                ReceivedAt = timestamp,
                SourceId = _config.SourceId,
                ChannelId = channelId,
                AuthorId = userId,
                AuthorLogin = userLogin,
                Content = message.Content!.Text
            };

            await ctx.Messages.AddAsync(messageEntity);
            await ctx.SaveChangesAsync();
        }
    }
}
