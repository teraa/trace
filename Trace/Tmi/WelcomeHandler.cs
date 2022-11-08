using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Teraa.Irc;
using Teraa.Twitch.Tmi;
using Teraa.Twitch.Tmi.Notifications;
using Trace.Data;

namespace Trace.Tmi;

[UsedImplicitly]
public class WelcomeHandler : INotificationHandler<MessageReceived>
{
    private readonly TraceDbContext _ctx;
    private readonly ILogger<WelcomeHandler> _logger;
    private readonly TmiService _tmi;

    public WelcomeHandler(TraceDbContext ctx, ILogger<WelcomeHandler> logger, TmiService tmi)
    {
        _ctx = ctx;
        _logger = logger;
        _tmi = tmi;
    }

    public async Task Handle(MessageReceived notification, CancellationToken cancellationToken)
    {
        if (notification.Message is not {Command: (Command) 1}) return;

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
