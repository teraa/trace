﻿using JetBrains.Annotations;
using MediatR;
using Teraa.Twitch.PubSub.Messages.ChatModeratorActions;
using Teraa.Twitch.PubSub.Notifications;
using Trace.Data;
using Trace.Features.Users;
using Timeout = Teraa.Twitch.PubSub.Messages.ChatModeratorActions.Timeout;


namespace Trace.PubSub;

[UsedImplicitly]
public class ChatModeratorActionReceivedHandler : INotificationHandler<ChatModeratorActionReceived>
{
    private readonly AppDbContext _ctx;
    private readonly ILogger<ChatModeratorActionReceivedHandler> _logger;
    private readonly ISender _sender;

    public ChatModeratorActionReceivedHandler(AppDbContext ctx, ILogger<ChatModeratorActionReceivedHandler> logger, ISender sender)
    {
        _ctx = ctx;
        _logger = logger;
        _sender = sender;
    }

    public async Task Handle(ChatModeratorActionReceived notification, CancellationToken cancellationToken)
    {
        var entity = new Data.Models.Pubsub.ModeratorAction
        {
            Timestamp = notification.ReceivedAt,
            ChannelId = notification.Topic.ChannelId,
            Action = notification.Action.Action,
            InitiatorId = notification.Action.InitiatorId,
        };

        var userUpdates = new List<UpdateUser.Command>(2);

        if (notification.Action is IInitiatorModeratorAction initiatorAction)
        {
            entity.InitiatorName = initiatorAction.Initiator.Login;

            userUpdates.Add(new UpdateUser.Command(
                initiatorAction.Initiator.Id,
                initiatorAction.Initiator.Login,
                notification.ReceivedAt));
        }

        if (notification.Action is ITargetedModeratorAction targetedAction)
        {
            entity.TargetId = targetedAction.Target.Id;
            entity.TargetName = targetedAction.Target.Login;

            userUpdates.Add(new UpdateUser.Command(
                targetedAction.Target.Id,
                targetedAction.Target.Login,
                notification.ReceivedAt));
        }

        switch (notification.Action)
        {
            case Ban x:
                entity.Reason = x.Reason;
                break;
            case Delete x:
                entity.MessageId = x.MessageId;
                entity.Message = x.Message;
                break;
            case Followers x:
                entity.Duration = x.Duration;
                break;
            case Raid x:
                entity.TargetName = x.TargetDisplayName;
                entity.InitiatorName = x.InitiatorDisplayName;
                break;
            case Unraid x:
                entity.InitiatorName = x.InitiatorDisplayName;
                break;
            case Slow x:
                entity.Duration = x.Duration;
                break;
            case Timeout x:
                entity.Duration = x.Duration;
                entity.Reason = x.Reason;
                break;
            case ApproveUnbanRequest x:
                entity.ModeratorMessage = x.ModeratorMessage;
                break;
            case DenyUnbanRequest x:
                entity.ModeratorMessage = x.ModeratorMessage;
                break;
            case ITermModeratorAction x:
                entity.TermId = x.Id;
                entity.TermText = x.Text;
                entity.UpdatedAt = x.UpdatedAt;
                break;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (entity.InitiatorName is null)
        {
            _logger.LogWarning("InitiatorName not set for {@Action}", notification.Action);
        }

        _ctx.ModeratorActions.Add(entity);
        await _ctx.SaveChangesAsync(cancellationToken);

        foreach (var update in userUpdates)
        {
            await _sender.Send(update, cancellationToken);
        }
    }
}
