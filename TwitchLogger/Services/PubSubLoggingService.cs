using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Twitch.PubSub;
using TwitchLogger.Data;

namespace TwitchLogger.Services
{
    class PubSubLoggingConfig
    {

    }

    class PubSubLoggingService : IHostedService
    {
        private readonly TwitchPubSubClient _client;
        private readonly ILogger<PubSubClientService> _logger;
        private readonly IDbContextFactory<TwitchLoggerDbContext> _contextFactory;

        public PubSubLoggingService(
            TwitchPubSubClient client,
            ILogger<PubSubClientService> logger,
            IDbContextFactory<TwitchLoggerDbContext> contextFactory)
        {
            _client = client;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.ModeratorActionReceived += ModeratorActionReceivedAsync;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.ModeratorActionReceived -= ModeratorActionReceivedAsync;

            return Task.CompletedTask;
        }

        private async Task ModeratorActionReceivedAsync(Topic topic, ModeratorAction action)
        {
            using var ctx = _contextFactory.CreateDbContext();

            var now = DateTimeOffset.UtcNow;

            if (action.Moderator is not null)
                await ctx.CreateOrUpdateUserAsync(new Data.Models.Twitch.User { Id = action.Moderator.Id, Login = action.Moderator.Login, FirstSeenAt = now });

            if (action.Target is not null)
                await ctx.CreateOrUpdateUserAsync(new Data.Models.Twitch.User { Id = action.Target.Id, Login = action.Target.Login, FirstSeenAt = now });

            var actionEntity = new Data.Models.Twitch.ModeratorAction
            {
                CreatedAt = now,
                ChannelId = action.ChannelId,
                Action = action.Action,
                Args = action.Args?.ToList(),
                MessageId = action.MessageId,
                ModeratorId = action.Moderator?.Id,
                TargetId = action.Target?.Id,
                TargetLogin = action.Target?.Login,
                ModeratorMessage = action.ModeratorMessage,
            };

            ctx.ModeratorActions.Add(actionEntity);
            await ctx.SaveChangesAsync();
        }
    }
}
