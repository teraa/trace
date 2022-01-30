using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IrcMessageParser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twitch.Irc;
using TwitchLogger.Data;
using TwitchLogger.Options;

namespace TwitchLogger.Services;

class ChatService : IHostedService
{
    private readonly TwitchIrcClient _client;
    private readonly ILogger<ChatService> _logger;
    private readonly IDbContextFactory<TwitchLoggerDbContext> _contextFactory;
    private readonly ChatOptions _options;
    private short _messageSourceId;


    public ChatService(
        TwitchIrcClient client,
        ILogger<ChatService> logger,
        IDbContextFactory<TwitchLoggerDbContext> contextFactory,
        IOptions<ChatOptions> options)
    {
        _client = client;
        _logger = logger;
        _contextFactory = contextFactory;
        _options = options.Value;

        _client.Connected += ConnectedAsync;
        _client.IrcMessageReceived += IrcMessageReceivedAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options.MessageSourceName is null)
            throw new ArgumentNullException(nameof(_options.MessageSourceName));

        using (var ctx = _contextFactory.CreateDbContext())
        {
            var source = await ctx.MessageSources
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == _options.MessageSourceName, cancellationToken);

            if (source is null)
            {
                source = new Data.Models.Twitch.MessageSource { Name = _options.MessageSourceName };
                ctx.MessageSources.Add(source);
                await ctx.SaveChangesAsync();
            }

            _messageSourceId = source.Id;

            _logger.LogInformation($"Using message source: {source.Name} ({source.Id})");
        }

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
                .AsNoTracking()
                .Select(x => x.Channel.Login)
                .ToArrayAsync();
        }

        var tasks = new List<Task>();
        foreach (var channelLogin in channelLogins)
            tasks.Add(_client.SendAsync(new IrcMessage { Command = IrcCommand.JOIN, Content = new($"#{channelLogin}") }).AsTask());

        await Task.WhenAll(tasks);

        _logger.LogInformation($"Joined {channelLogins.Length} channels: {string.Join(", ", channelLogins)}");
    }

    private async ValueTask IrcMessageReceivedAsync(IrcMessage message)
    {
        if (message.Command != IrcCommand.PRIVMSG) return;

        var channelLogin = message.Arg![1..];
        var channelId = message.Tags!["room-id"];
        var userLogin = message.Prefix!.Name;
        var userId = message.Tags!["user-id"];
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(message.Tags["tmi-sent-ts"]));

        using var ctx = _contextFactory.CreateDbContext();

        await ctx.CreateOrUpdateUserAsync(new Data.Models.Twitch.User
        {
            Id = channelId,
            Login = channelLogin,
            FirstSeenAt = timestamp
        });
        await ctx.CreateOrUpdateUserAsync(new Data.Models.Twitch.User
        {
            Id = userId,
            Login = userLogin,
            FirstSeenAt = timestamp
        });

        var messageEntity = new Data.Models.Twitch.Message
        {
            ReceivedAt = timestamp,
            SourceId = _messageSourceId,
            ChannelId = channelId,
            AuthorId = userId,
            AuthorLogin = userLogin,
            Content = message.Content!.Text
        };

        ctx.Messages.Add(messageEntity);
        await ctx.SaveChangesAsync();
    }
}
