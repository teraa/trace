using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Teraa.Irc;
using Teraa.Twitch.Tmi;
using Teraa.Twitch.Tmi.Notifications;
using TwitchLogger.Data;

namespace TwitchLogger.Services;

public class ConnectedHandler : INotificationHandler<Connected>
{
    private readonly TmiService _tmi;
    private readonly TwitchLoggerDbContext _ctx;
    private readonly ILogger<ConnectedHandler> _logger;

    public ConnectedHandler(
        TmiService tmi,
        TwitchLoggerDbContext ctx,
        ILogger<ConnectedHandler> logger)
    {
        _tmi = tmi;
        _ctx = ctx;
        _logger = logger;
    }

    public async Task Handle(Connected notification, CancellationToken cancellationToken)
    {
        _tmi.EnqueueMessage(new Message
        {
            Command = Command.NICK,
            Content = new("justinfan1"),
        });

        _tmi.EnqueueMessage(new Message
        {
            Command = Command.CAP,
            Arg = "REQ",
            Content = new("twitch.tv/tags twitch.tv/commands")
        });

        var channels = await _ctx.ChatLogs
            .Select(x => x.Channel.Login)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Joining: {Channels}", channels);

        foreach (string channel in channels)
            _tmi.EnqueueMessage(new Message
            {
                Command = Command.JOIN,
                Content = new($"#{channel}"),
            });
    }
}

public class MessageHandler : INotificationHandler<MessageReceived>
{
    private readonly TwitchLoggerDbContext _ctx;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;
    private readonly IMemoryCache _cache;

    public MessageHandler(
        TwitchLoggerDbContext ctx,
        MemoryCacheEntryOptions cacheEntryOptions,
        IMemoryCache cache)
    {
        _ctx = ctx;
        _cacheEntryOptions = cacheEntryOptions;
        _cache = cache;
    }

    public async Task Handle(MessageReceived notification, CancellationToken cancellationToken)
    {
        if (notification.Message.Command != Command.PRIVMSG) return;

        // Debug.Assert(notification.Message is {Arg: not null, Tags: not null, Prefix: not null, Content: not null});

        Debug.Assert(notification.Message.Arg is not null);
        Debug.Assert(notification.Message.Tags is not null);
        Debug.Assert(notification.Message.Prefix is not null);
        Debug.Assert(notification.Message.Content is not null);

        var channelLogin = notification.Message.Arg[1..];
        var channelId = notification.Message.Tags["room-id"];
        var userLogin = notification.Message.Prefix.Name;
        var userId = notification.Message.Tags["user-id"];
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(
            long.Parse(notification.Message.Tags["tmi-sent-ts"]));

        await _ctx.TryUpdateUserAsync(channelId, channelLogin, timestamp, _cache, _cacheEntryOptions, cancellationToken);
        await _ctx.TryUpdateUserAsync(userId, userLogin, timestamp, _cache, _cacheEntryOptions, cancellationToken);

        var messageEntity = new Data.Models.Twitch.Message
        {
            ReceivedAt = timestamp,
            SourceId = _cache.Get<short>("source_id"),
            ChannelId = channelId,
            AuthorId = userId,
            AuthorLogin = userLogin,
            Content = notification.Message.Content.Text
        };

        _ctx.Messages.Add(messageEntity);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
