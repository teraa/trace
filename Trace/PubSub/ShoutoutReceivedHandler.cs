using JetBrains.Annotations;
using MediatR;
using Teraa.Twitch.PubSub.Notifications;
using Trace.Data;
using Trace.Data.Models.Pubsub;
using Trace.Features.Users;

namespace Trace.PubSub;

[UsedImplicitly]
public sealed class ShoutoutReceivedHandler : INotificationHandler<ShoutoutReceived>
{
    private readonly AppDbContext _ctx;
    private readonly ISender _sender;

    public ShoutoutReceivedHandler(AppDbContext ctx, ISender sender)
    {
        _ctx = ctx;
        _sender = sender;
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

        await _sender.Send(new UpdateUser.Command(
            notification.Shoutout.SourceUserId,
            notification.Shoutout.SourceLogin,
            notification.ReceivedAt), cancellationToken);

        await _sender.Send(new UpdateUser.Command(
            notification.Shoutout.TargetUserId,
            notification.Shoutout.TargetLogin,
            notification.ReceivedAt), cancellationToken);
    }
}
