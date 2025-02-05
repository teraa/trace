﻿using JetBrains.Annotations;
using MediatR;
using Teraa.Twitch.PubSub.Notifications;
using Trace.Common;
using Trace.Data;
using Trace.Data.Models.Pubsub;

namespace Trace.PubSub;

[UsedImplicitly]
public sealed class ShoutoutReceivedHandler : INotificationHandler<ShoutoutReceived>
{
    private readonly AppDbContext _ctx;
    private readonly UpdateUsers.Handler _updateUserHandler;

    public ShoutoutReceivedHandler(AppDbContext ctx, UpdateUsers.Handler updateUserHandler)
    {
        _ctx = ctx;
        _updateUserHandler = updateUserHandler;
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

        await _updateUserHandler.HandleAsync(new UpdateUsers.Command(
                [
                    new UpdateUsers.Command.User(
                        notification.Shoutout.SourceUserId,
                        notification.Shoutout.SourceLogin
                    ),
                    new UpdateUsers.Command.User(
                        notification.Shoutout.TargetUserId,
                        notification.Shoutout.TargetLogin
                    ),
                ],
                notification.ReceivedAt),
            cancellationToken);
    }
}
