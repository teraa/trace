using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Teraa.Irc;
using Teraa.Twitch.Tmi;
using Teraa.Twitch.Tmi.Notifications;
using Trace.Data;

namespace Trace.Tmi;

[UsedImplicitly]
public class ConnectedHandler : INotificationHandler<Connected>
{
    private readonly TmiService _tmi;
    private readonly TraceDbContext _ctx;
    private readonly ILogger<ConnectedHandler> _logger;

    public ConnectedHandler(
        TmiService tmi,
        TraceDbContext ctx,
        ILogger<ConnectedHandler> logger)
    {
        _tmi = tmi;
        _ctx = ctx;
        _logger = logger;
    }

    public async Task Handle(Connected notification, CancellationToken cancellationToken)
    {
        _tmi.EnqueueMessage(new Message(
            Command: Command.NICK,
            Content: new Content("justinfan1")));

        _tmi.EnqueueMessage(new Message(
            Command: Command.CAP,
            Arg: "REQ",
            Content: new Content("twitch.tv/tags twitch.tv/commands")));

        var channels = await _ctx.TmiConfigs
            .Select(x => x.ChannelLogin)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Joining: {Channels}", channels);

        foreach (string channel in channels)
            _tmi.EnqueueMessage(new Message(
                Command: Command.JOIN,
                Content: new Content($"#{channel}")));
    }
}
