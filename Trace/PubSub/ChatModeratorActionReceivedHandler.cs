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
    private readonly ILogger<ChatModeratorActionReceivedHandler> _logger;

    public ChatModeratorActionReceivedHandler(TraceDbContext ctx, ILogger<ChatModeratorActionReceivedHandler> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async Task Handle(ChatModeratorActionReceived notification, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow;

        var entity = new Data.Models.Pubsub.ModeratorAction
        {
            Timestamp = timestamp,
            ChannelId = notification.Topic.ChannelId,
            Action = notification.Action.Action,
            InitiatorId = notification.Action.InitiatorId,
        };

        if (notification.Action is ITargetedModeratorAction targetedAction)
        {
            entity.TargetId = targetedAction.Target.Id;
            entity.TargetName = targetedAction.Target.Login;
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

        // InitiatorName handling to log any occurrences where it's not set
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
