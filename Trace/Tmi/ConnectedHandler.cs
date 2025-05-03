using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Teraa.Irc;
using Teraa.Twitch.Tmi;

namespace Trace.Tmi;

[UsedImplicitly]
public class ConnectedHandler : ITmiEventHandler<ConnectedEvent>
{
    private readonly IOptionsMonitor<TmiOptions> _options;

    public ConnectedHandler(IOptionsMonitor<TmiOptions> options)
    {
        _options = options;
    }

    public ValueTask HandleAsync(ConnectedEvent evt, CancellationToken cancellationToken)
    {
        evt.Service.EnqueueMessage(new Message(
            Command.PASS,
            Content: new Content(_options.CurrentValue.Token)));

        evt.Service.EnqueueMessage(new Message(
            Command: Command.NICK,
            Content: new Content(_options.CurrentValue.Login)));

        evt.Service.EnqueueMessage(new Message(
            Command: Command.CAP,
            Arg: "REQ",
            Content: new Content("twitch.tv/tags twitch.tv/commands")));

        return ValueTask.CompletedTask;
    }
}
