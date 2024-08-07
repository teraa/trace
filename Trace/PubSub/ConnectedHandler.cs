﻿using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Teraa.Twitch.PubSub;
using Teraa.Twitch.PubSub.Notifications;
using Teraa.Twitch.PubSub.Payloads;
using Trace.Data;

namespace Trace.PubSub;

[UsedImplicitly]
public class ConnectedHandler : INotificationHandler<Connected>
{
    private readonly IPubSubClient _pubSub;
    private readonly AppDbContext _ctx;
    private readonly ILogger<ConnectedHandler> _logger;
    private readonly IOptionsMonitor<PubSubOptions> _options;

    public ConnectedHandler(
        IPubSubClient pubSub,
        AppDbContext ctx,
        ILogger<ConnectedHandler> logger,
        IOptionsMonitor<PubSubOptions> options)
    {
        _pubSub = pubSub;
        _ctx = ctx;
        _logger = logger;
        _options = options;
    }

    public async Task Handle(Connected notification, CancellationToken cancellationToken)
    {
        var topics = await _ctx.PubsubConfigs
            .Select(x => x.Topic)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Joining: {Topics}", topics);

        foreach (string topic in topics)
            _pubSub.EnqueueMessage(Payload.CreateListen([topic], _options.CurrentValue.Token, topic));
    }
}
