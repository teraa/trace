using System;
using System.Threading;
using System.Threading.Tasks;
using IrcMessageParser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twitch.Irc;
using TwitchLogger.Data;
using TwitchLogger.Data.Models.Twitch;
using TwitchLogger.Options;

namespace TwitchLogger.Services
{
    class ChatLoggingService : IHostedService
    {
        private readonly TwitchIrcClient _client;
        private readonly ILogger<ChatLoggingService> _logger;
        private readonly IDbContextFactory<TwitchLoggerDbContext> _contextFactory;
        private readonly ChatOptions _options;
        private MessageSource _source = null!;

        public ChatLoggingService(
            TwitchIrcClient client,
            ILogger<ChatLoggingService> logger,
            IDbContextFactory<TwitchLoggerDbContext> contextFactory,
            IOptions<ChatOptions> options)
        {
            _client = client;
            _logger = logger;
            _contextFactory = contextFactory;
            _options = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.MessageSourceName is null)
                throw new ArgumentNullException(nameof(_options.MessageSourceName));

            using (var ctx = _contextFactory.CreateDbContext())
            {
                var source = await ctx.MessageSources.FirstOrDefaultAsync(x => x.Name == _options.MessageSourceName, cancellationToken);
                if (source is null)
                {
                    source = new MessageSource { Name = _options.MessageSourceName };
                    ctx.MessageSources.Add(source);
                    await ctx.SaveChangesAsync();
                }

                _source = source;
            }

            _logger.LogInformation($"Using message source: {_source.Name} ({_source.Id})");

            _client.IrcMessageReceived += IrcMessageReceivedAsync;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.IrcMessageReceived -= IrcMessageReceivedAsync;

            return Task.CompletedTask;
        }

        private async ValueTask IrcMessageReceivedAsync(IrcMessage message)
        {
            if (message.Command != IrcCommand.PRIVMSG) return;

            var channelLogin = message.Arg![1..];
            var channelId =  message.Tags!["room-id"];
            var userLogin = message.Prefix!.Name;
            var userId = message.Tags!["user-id"];
            var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(message.Tags["tmi-sent-ts"]));

            using var ctx = _contextFactory.CreateDbContext();

            await ctx.CreateOrUpdateUserAsync(new User { Id = channelId, Login = channelLogin, FirstSeenAt = timestamp });
            await ctx.CreateOrUpdateUserAsync(new User { Id = userId, Login = userLogin, FirstSeenAt = timestamp });

            var messageEntity = new Message
            {
                ReceivedAt = timestamp,
                SourceId = _source.Id,
                ChannelId = channelId,
                AuthorId = userId,
                AuthorLogin = userLogin,
                Content = message.Content!.Text
            };

            ctx.Messages.Add(messageEntity);
            await ctx.SaveChangesAsync();
        }
    }
}
