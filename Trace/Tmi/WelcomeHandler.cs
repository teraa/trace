using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Teraa.Irc;
using Teraa.Twitch.Tmi;
using Trace.Data;

namespace Trace.Tmi;

[UsedImplicitly]
public class WelcomeHandler : ITmiEventHandler<MessageReceivedEvent>
{
    private readonly AppDbContext _ctx;
    private readonly ILogger<WelcomeHandler> _logger;

    public WelcomeHandler(
        AppDbContext ctx,
        ILogger<WelcomeHandler> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async ValueTask HandleAsync(MessageReceivedEvent evt, CancellationToken cancellationToken)
    {
        if (evt.Message is not {Command: (Command) 1}) return;

        var channels = await _ctx.TmiConfigs
            .Select(x => x.ChannelLogin)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Joining: {Channels}", channels);

        foreach (string channel in channels)
            evt.Service.EnqueueMessage(new Message(
                Command: Command.JOIN,
                Content: new Content($"#{channel}")));
    }
}
