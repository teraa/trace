using System.Globalization;
using JetBrains.Annotations;
using Teraa.Irc;
using Teraa.Twitch.Tmi;
using Trace.Common;
using Trace.Data;

namespace Trace.Tmi;

[UsedImplicitly]
public class MessageHandler : ITmiEventHandler<MessageReceivedEvent>
{
    private readonly AppDbContext _ctx;
    private readonly ISourceProvider _sourceProvider;
    private readonly ILogger<MessageHandler> _logger;
    private readonly UpdateUsers.Handler _updateUserHandler;

    public MessageHandler(
        AppDbContext ctx,
        ISourceProvider sourceProvider,
        ILogger<MessageHandler> logger,
        UpdateUsers.Handler updateUserHandler)
    {
        _ctx = ctx;
        _sourceProvider = sourceProvider;
        _logger = logger;
        _updateUserHandler = updateUserHandler;
    }

    public async ValueTask HandleAsync(MessageReceivedEvent evt, CancellationToken cancellationToken)
    {
        if (evt.Message.Command != Command.PRIVMSG) return;

        if (evt.Message is not
            {
                Arg: not null,
                Tags: not null,
                Prefix: not null,
                Content: not null,
            })
        {
            _logger.LogWarning("Invalid message received: {Message}", evt.Message);
            return;
        }

        // string channelLogin = notification.Message.Arg[1..];
        string channelId = evt.Message.Tags["room-id"];
        string userLogin = evt.Message.Prefix.Name;
        string userId = evt.Message.Tags["user-id"];
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(
            long.Parse(evt.Message.Tags["tmi-sent-ts"], CultureInfo.InvariantCulture));

        var messageEntity = new Data.Models.Tmi.Message
        {
            Timestamp = timestamp,
            SourceId = _sourceProvider.SourceId,
            ChannelId = channelId,
            AuthorId = userId,
            AuthorLogin = userLogin,
            Content = evt.Message.Content.Text
        };

        _ctx.TmiMessages.Add(messageEntity);
        await _ctx.SaveChangesAsync(cancellationToken);
        await _updateUserHandler.HandleAsync(new UpdateUsers.Command(
                [new UpdateUsers.Command.User(userId, userLogin)],
                timestamp),
            cancellationToken);
    }
}
