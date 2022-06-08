using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Options;
using Teraa.Irc;
using Teraa.Twitch.Tmi.Notifications;
using TwitchLogger.Data;

namespace TwitchLogger.Tmi;

[UsedImplicitly]
public class MessageHandler : INotificationHandler<MessageReceived>
{
    private readonly TwitchLoggerDbContext _ctx;
    private readonly SourceCache _cache;
    private readonly TmiOptions _options;
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(
        TwitchLoggerDbContext ctx,
        SourceCache cache,
        IOptions<TmiOptions> options,
        ILogger<MessageHandler> logger)
    {
        _ctx = ctx;
        _cache = cache;
        _logger = logger;
        _options = options.Value;
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

        short sourceId = await _cache.GetOrLoadSourceIdAsync(_options.MessageSourceName, cancellationToken);

        var messageEntity = new Data.Models.Tmi.Message
        {
            Timestamp = timestamp,
            SourceId = sourceId,
            ChannelId = channelId,
            AuthorId = userId,
            AuthorLogin = userLogin,
            Content = notification.Message.Content.Text
        };

        _ctx.TmiMessages.Add(messageEntity);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
