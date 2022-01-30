using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twitch.PubSub;
using TwitchLogger.Data;
using TwitchLogger.Options;

namespace TwitchLogger.Services;

    class PubSubService : IHostedService
    {
        private readonly TwitchPubSubClient _client;
        private readonly ILogger<PubSubService> _logger;
        private readonly PubSubOptions _options;
        private readonly IDbContextFactory<TwitchLoggerDbContext> _contextFactory;

        public PubSubService(
            TwitchPubSubClient client,
            ILogger<PubSubService> logger,
            IOptions<PubSubOptions> options,
            IDbContextFactory<TwitchLoggerDbContext> contextFactory)
        {
            _client = client;
            _logger = logger;
            _options = options.Value;
            _contextFactory = contextFactory;

            _client.Connected += ConnectedAsync;
            _client.ModeratorActionReceived += ModeratorActionReceivedAsync;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.ConnectAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();
        }

        private async ValueTask ConnectedAsync()
        {
            Topic[] topics;

            using (var ctx = _contextFactory.CreateDbContext())
            {
                var topicsNames = await ctx.PubSubLogs
                    .AsNoTracking()
                    .Select(x => x.Topic)
                    .ToArrayAsync();

                topics = topicsNames.Select(Topic.Parse).ToArray();
            }

            var tasks = new List<Task<PubSubMessage>>();
            foreach (var topic in topics)
                tasks.Add(_client.ListenAsync(topic, _options.Token));

            var responses = await Task.WhenAll(tasks);

            foreach ((Topic topic, PubSubMessage response) in topics.Zip(responses))
            {
                if (response.Error is { Length: > 0 })
                    _logger.LogError($"Error listening {topic}: {response.Error}.");
                else
                    _logger.LogInformation($"Successfully listened {topic}.");
            }
        }

        private async ValueTask ModeratorActionReceivedAsync(Topic topic, ModeratorAction action)
        {
            using var ctx = _contextFactory.CreateDbContext();

            var now = DateTimeOffset.UtcNow;

            if (action.Moderator is not null)
                await ctx.CreateOrUpdateUserAsync(new Data.Models.Twitch.User
                {
                    Id = action.Moderator.Id,
                    Login = action.Moderator.Login,
                    FirstSeenAt = now
                });

            if (action.Target is not null)
                await ctx.CreateOrUpdateUserAsync(new Data.Models.Twitch.User
                {
                    Id = action.Target.Id,
                    Login = action.Target.Login,
                    FirstSeenAt = now
                });

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
