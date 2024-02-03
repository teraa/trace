using JetBrains.Annotations;
using MediatR;
using Teraa.Twitch.PubSub.Notifications;
using Trace.Data;
using Trace.Data.Models.Pubsub;

namespace Trace.PubSub;

[UsedImplicitly]
public sealed class ShoutoutReceivedHandler : INotificationHandler<ShoutoutReceived>
{
    private readonly TraceDbContext _ctx;

    public ShoutoutReceivedHandler(TraceDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task Handle(ShoutoutReceived notification, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow;

        var entity = new ModeratorAction
        {
            Action = "shoutout",
            ChannelId = notification.Topic.ChannelId,
            InitiatorId = notification.Shoutout.SourceUserId,
            InitiatorName = notification.Shoutout.SourceLogin,
            Timestamp = timestamp,
            TargetId = notification.Shoutout.TargetUserId,
            TargetName = notification.Shoutout.TargetLogin,
        };

        _ctx.ModeratorActions.Add(entity);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
