using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    private readonly IOptionsMonitor<TmiOptions> _options;

    public ConnectedHandler(
        TmiService tmi,
        TraceDbContext ctx,
        ILogger<ConnectedHandler> logger,
        IOptionsMonitor<TmiOptions> options)
    {
        _tmi = tmi;
        _ctx = ctx;
        _logger = logger;
        _options = options;
    }

    public async Task Handle(Connected notification, CancellationToken cancellationToken)
    {
        _tmi.EnqueueMessage(new Message(
            Command.PASS,
            Content: new Content(_options.CurrentValue.Token)));

        _tmi.EnqueueMessage(new Message(
            Command: Command.NICK,
            Content: new Content(_options.CurrentValue.Login)));

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
