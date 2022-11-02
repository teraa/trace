using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Options;
using Teraa.Irc;
using Teraa.Twitch.Tmi;
using Teraa.Twitch.Tmi.Notifications;
using Trace.Data;
using Trace.Options;

namespace Trace.Tmi;

[UsedImplicitly]
public class ConnectedHandler : INotificationHandler<Connected>
{
    private readonly TmiService _tmi;
    private readonly IOptionsMonitor<TmiOptions> _options;

    public ConnectedHandler(
        TmiService tmi,
        TraceDbContext ctx,
        ILogger<ConnectedHandler> logger,
        IOptionsMonitor<TmiOptions> options)
    {
        _tmi = tmi;
        _options = options;
    }

    public Task Handle(Connected notification, CancellationToken cancellationToken)
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

        return Task.CompletedTask;
    }
}
