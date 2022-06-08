using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Teraa.Twitch.PubSub.Messages.ChatModeratorActions;
using Teraa.Twitch.PubSub.Notifications;
using TwitchLogger.Data;
using Timeout = Teraa.Twitch.PubSub.Messages.ChatModeratorActions.Timeout;


namespace TwitchLogger.PubSub;

[UsedImplicitly]
public class ChatModeratorActionReceivedHandler : INotificationHandler<ChatModeratorActionReceived>
{
    private readonly TwitchLoggerDbContext _ctx;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

    public ChatModeratorActionReceivedHandler(TwitchLoggerDbContext ctx,
        IMemoryCache cache,
        MemoryCacheEntryOptions cacheEntryOptions)
    {
        _ctx = ctx;
        _cache = cache;
        _cacheEntryOptions = cacheEntryOptions;
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

            await _ctx.TryUpdateUserAsync(
                id: evt.TargetId,
                login: evt.Target,
                timestamp: timestamp,
                cache: _cache,
                options: _cacheEntryOptions,
                cancellationToken: cancellationToken);
        }

        action.Timestamp = timestamp;
        action.ChannelId = notification.Topic.ChannelId;
        action.Action = notification.Action.Action;
        action.InitiatorId = notification.Action.InitiatorId;
        action.InitiatorName = notification.Action.Initiator;

        if (notification.Action is not (Raid or Unraid))
        {
            await _ctx.TryUpdateUserAsync(
                id: notification.Action.InitiatorId,
                login: notification.Action.Initiator,
                timestamp: timestamp,
                cache: _cache,
                options: _cacheEntryOptions,
                cancellationToken: cancellationToken);
        }

        _ctx.ModeratorActions.Add(action);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
