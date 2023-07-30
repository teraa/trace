﻿using JetBrains.Annotations;
using MediatR;
using Teraa.Twitch.PubSub.Messages.ChatModeratorActions;
using Teraa.Twitch.PubSub.Notifications;
using Trace.Data;
using Timeout = Teraa.Twitch.PubSub.Messages.ChatModeratorActions.Timeout;


namespace Trace.PubSub;

[UsedImplicitly]
public class ChatModeratorActionReceivedHandler : INotificationHandler<ChatModeratorActionReceived>
{
    private readonly TraceDbContext _ctx;
    private readonly ILogger<ChatModeratorActionReceivedHandler> _logger;

    public ChatModeratorActionReceivedHandler(TraceDbContext ctx, ILogger<ChatModeratorActionReceivedHandler> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async Task Handle(ChatModeratorActionReceived notification, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow;

        var entity = notification.Action switch
        {
            Ban x => new Data.Models.Pubsub.Ban
            {
                Reason = x.Reason,
            },

            Delete x => new Data.Models.Pubsub.Delete
            {
                MessageId = x.MessageId,
                Message = x.Message,
            },

            Followers x => new Data.Models.Pubsub.Followers
            {
                Duration = x.Duration,
            },

            Raid x => new Data.Models.Pubsub.Raid
            {
                TargetName = x.TargetDisplayName,
            },

            Slow x => new Data.Models.Pubsub.Slow
            {
                Duration = x.Duration,
            },

            Timeout x => new Data.Models.Pubsub.Timeout
            {
                Duration = x.Duration,
                Reason = x.Reason,
            },

            ApproveUnbanRequest x => new Data.Models.Pubsub.UnbanRequestAction
            {
                ModeratorMessage = x.ModeratorMessage,
            },

            DenyUnbanRequest x => new Data.Models.Pubsub.UnbanRequestAction
            {
                ModeratorMessage = x.ModeratorMessage,
            },

            ITargetedModeratorAction => new Data.Models.Pubsub.TargetedModeratorAction(),

            ITermModeratorAction x => new Data.Models.Pubsub.TermAction
            {
                TermId = x.Id,
                Text = x.Text,
                UpdatedAt = x.UpdatedAt,
            },

            _ => new Data.Models.Pubsub.ModeratorAction()
        };

        if (notification.Action is ITargetedModeratorAction targetedAction)
        {
            var targetedEntity = (Data.Models.Pubsub.TargetedModeratorAction) entity;
            targetedEntity.TargetId = targetedAction.Target.Id;
            targetedEntity.TargetName = targetedAction.Target.Login;
        }

        entity.Timestamp = timestamp;
        entity.ChannelId = notification.Topic.ChannelId;
        entity.Action = notification.Action.Action;
        entity.InitiatorId = notification.Action.InitiatorId;

        switch (notification.Action)
        {
            case IInitiatorModeratorAction x:
                entity.InitiatorName = x.Initiator.Login;
                break;
            case Raid x:
                entity.InitiatorName = x.InitiatorDisplayName;
                break;
            case Unraid x:
                entity.InitiatorName = x.InitiatorDisplayName;
                break;
            default:
                _logger.LogWarning("InitiatorName not set for {@Action}", notification.Action);
                break;
        }

        _ctx.ModeratorActions.Add(entity);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
