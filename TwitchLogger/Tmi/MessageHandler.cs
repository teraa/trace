using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Teraa.Irc;
using Teraa.Twitch.Tmi.Notifications;
using TwitchLogger.Data;

namespace TwitchLogger.Tmi;

[UsedImplicitly]
public class MessageHandler : INotificationHandler<MessageReceived>
{
    private readonly TwitchLoggerDbContext _ctx;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(
        TwitchLoggerDbContext ctx,
        MemoryCacheEntryOptions cacheEntryOptions,
        IMemoryCache cache,
        ILogger<MessageHandler> logger)
    {
        _ctx = ctx;
        _cacheEntryOptions = cacheEntryOptions;
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(MessageReceived notification, CancellationToken cancellationToken)
    {
        if (notification.Message.Command != Command.PRIVMSG) return;

        if (notification.Message is not
            {
                Arg: not null,
                Tags: not null,
                Prefix: not null,
                Content: not null
            })
        {
            _logger.LogWarning("Invalid message received: {Message}", notification.Message);
            return;
        }

        string channelLogin = notification.Message.Arg[1..];
        string channelId = notification.Message.Tags["room-id"];
        string userLogin = notification.Message.Prefix.Name;
        string userId = notification.Message.Tags["user-id"];
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
