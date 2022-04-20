using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Teraa.Irc;
using Teraa.Twitch.Tmi;
using Teraa.Twitch.Tmi.Notifications;
using TwitchLogger.Data;

namespace TwitchLogger.Tmi;

[UsedImplicitly]
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
            Content = new Content("justinfan1"),
        });

        _tmi.EnqueueMessage(new Message
        {
            Command = Command.CAP,
            Arg = "REQ",
            Content = new Content("twitch.tv/tags twitch.tv/commands")
        });

        var channels = await _ctx.ChatLogs
            .Select(x => x.Channel.Login)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Joining: {Channels}", channels);

        foreach (string channel in channels)
            _tmi.EnqueueMessage(new Message
            {
                Command = Command.JOIN,
                Content = new Content($"#{channel}"),
            });
    }
}
