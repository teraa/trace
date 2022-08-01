using JetBrains.Annotations;
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

    public ChatModeratorActionReceivedHandler(TraceDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task Handle(ChatModeratorActionReceived notification, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow;

        var action = notification.Action switch
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
                TargetName = x.Target,
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

        if (notification.Action is ITargetedModeratorAction evt)
        {
            var act = (Data.Models.Pubsub.TargetedModeratorAction) action;
            act.TargetId = evt.TargetId;
            act.TargetName = evt.Target;
        }

        action.Timestamp = timestamp;
        action.ChannelId = notification.Topic.ChannelId;
        action.Action = notification.Action.Action;
        action.InitiatorId = notification.Action.InitiatorId;
        action.InitiatorName = notification.Action.Initiator;

        // if (notification.Action is not (Raid or Unraid)) { }

        _ctx.ModeratorActions.Add(action);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
