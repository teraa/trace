using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Twitch.PubSub;
using TwitchLogger.Data;
using TwitchLogger.Options;

namespace TwitchLogger.Services;

internal class PubSubService : IHostedService
{
    private readonly TwitchPubSubClient _client;
    private readonly ILogger<PubSubService> _logger;
    private readonly PubSubOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

    public PubSubService(
        TwitchPubSubClient client,
        ILogger<PubSubService> logger,
        IOptions<PubSubOptions> options,
        IServiceScopeFactory scopeFactory,
        IMemoryCache cache,
        MemoryCacheEntryOptions cacheEntryOptions)
    {
        _client = client;
        _logger = logger;
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _cache = cache;
        _cacheEntryOptions = cacheEntryOptions;

        _client.Connected += ConnectedAsync;
        _client.ModeratorActionReceived += ModeratorActionReceivedAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.ConnectAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DisconnectAsync(cancellationToken);
    }

    private async ValueTask ConnectedAsync()
    {
        Topic[] topics;

        using (var scope = _scopeFactory.CreateScope())
        {
            var ctx = scope.Get<TwitchLoggerDbContext>();

            var topicsNames = await ctx.PubsubConfigs
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
                _logger.LogError("Error listening {Topic}: {Error}", topic, response.Error);
            else
                _logger.LogInformation("Successfully listened {Topic}", topic);
        }
    }

    private async ValueTask ModeratorActionReceivedAsync(Topic topic, ModeratorAction action)
    {
        using var scope = _scopeFactory.CreateScope();
        var ctx = scope.Get<TwitchLoggerDbContext>();

        var now = DateTimeOffset.UtcNow;

        if (action.Moderator is not null)
            await ctx.TryUpdateUserAsync(action.Moderator.Id, action.Moderator.Login, now, _cache, _cacheEntryOptions);

        if (action.Target is not null)
            await ctx.TryUpdateUserAsync(action.Target.Id, action.Target.Login, now, _cache, _cacheEntryOptions);

        var actionEntity = new Data.Models.Pubsub.ModeratorAction
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
