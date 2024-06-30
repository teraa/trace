using System.Globalization;
using JetBrains.Annotations;
using MediatR;
using Teraa.Irc;
using Teraa.Twitch.Tmi.Notifications;
using Trace.Data;
using Trace.Features.Users;

namespace Trace.Tmi;

[UsedImplicitly]
public class MessageHandler : INotificationHandler<MessageReceived>
{
    private readonly AppDbContext _ctx;
    private readonly ISourceProvider _sourceProvider;
    private readonly ILogger<MessageHandler> _logger;
    private readonly UpdateUser.Handler _updateUserHandler;

    public MessageHandler(
        AppDbContext ctx,
        ISourceProvider sourceProvider,
        ILogger<MessageHandler> logger,
        UpdateUser.Handler updateUserHandler)
    {
        _ctx = ctx;
        _sourceProvider = sourceProvider;
        _logger = logger;
        _updateUserHandler = updateUserHandler;
    }

    public async Task Handle(MessageReceived notification, CancellationToken cancellationToken)
    {
        if (notification.Message.Command != Command.PRIVMSG) return;

        if (notification.Message is not
            {
                Arg: not null,
                Tags: not null,
                Prefix: not null,
                Content: not null,
            })
        {
            _logger.LogWarning("Invalid message received: {Message}", notification.Message);
            return;
        }

        // string channelLogin = notification.Message.Arg[1..];
        string channelId = notification.Message.Tags["room-id"];
        string userLogin = notification.Message.Prefix.Name;
        string userId = notification.Message.Tags["user-id"];
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(
            long.Parse(notification.Message.Tags["tmi-sent-ts"], CultureInfo.InvariantCulture));

        var messageEntity = new Data.Models.Tmi.Message
        {
            Timestamp = timestamp,
            SourceId = _sourceProvider.SourceId,
            ChannelId = channelId,
            AuthorId = userId,
            AuthorLogin = userLogin,
            Content = notification.Message.Content.Text
        };

        _ctx.TmiMessages.Add(messageEntity);
        await _ctx.SaveChangesAsync(cancellationToken);
        await _updateUserHandler.HandleAsync(new UpdateUser.Command(userId, userLogin, timestamp), cancellationToken);
    }
}
