using JetBrains.Annotations;
using MediatR;
using Teraa.Twitch.PubSub.Notifications;
using Trace.Data;
using Trace.Data.Models.Pubsub;

namespace Trace.PubSub;

[UsedImplicitly]
public sealed class ShoutoutReceivedHandler : INotificationHandler<ShoutoutReceived>
{
    private readonly AppDbContext _ctx;

    public ShoutoutReceivedHandler(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task Handle(ShoutoutReceived notification, CancellationToken cancellationToken)
    {
        var entity = new ModeratorAction
        {
            Action = "shoutout",
            ChannelId = notification.Topic.ChannelId,
            InitiatorId = notification.Shoutout.SourceUserId,
            InitiatorName = notification.Shoutout.SourceLogin,
            Timestamp = notification.ReceivedAt,
            TargetId = notification.Shoutout.TargetUserId,
            TargetName = notification.Shoutout.TargetLogin,
        };

        _ctx.ModeratorActions.Add(entity);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
