using JetBrains.Annotations;
using MediatR;
using Teraa.Twitch.PubSub.Notifications;

namespace Trace.PubSub;

[UsedImplicitly]
public class ResponseReceivedHandler : INotificationHandler<ResponseReceived>
{
    private readonly ILogger<ResponseReceivedHandler> _logger;

    public ResponseReceivedHandler(ILogger<ResponseReceivedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ResponseReceived notification, CancellationToken cancellationToken)
    {
        if (notification.Error.Length > 0)
        {
            _logger.LogWarning("Received error response for {Topic}: {Error}", notification.Nonce, notification.Error);
            // TODO: Reconnect
        }

        return Task.CompletedTask;
    }
}
