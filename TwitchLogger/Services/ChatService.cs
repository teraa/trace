using IrcMessageParser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Twitch.Irc;
using TwitchLogger.Data;
using TwitchLogger.Options;

namespace TwitchLogger.Services;

class ChatService : IHostedService
{
    private readonly TwitchIrcClient _client;
    private readonly ILogger<ChatService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ChatOptions _options;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;
    private short _messageSourceId;

    public ChatService(
        TwitchIrcClient client,
        ILogger<ChatService> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<ChatOptions> options,
        IMemoryCache cache,
        MemoryCacheEntryOptions cacheEntryOptions)
    {
        _client = client;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _cache = cache;
        _cacheEntryOptions = cacheEntryOptions;

        _client.Connected += ConnectedAsync;
        _client.IrcMessageReceived += IrcMessageReceivedAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options.MessageSourceName is null)
            throw new ArgumentNullException(nameof(_options.MessageSourceName));

        using (var scope = _scopeFactory.CreateScope())
        {
            var ctx = scope.Get<TwitchLoggerDbContext>();

            var source = await ctx.MessageSources
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == _options.MessageSourceName, cancellationToken);

            if (source is null)
            {
                source = new Data.Models.Twitch.MessageSource { Name = _options.MessageSourceName };
                ctx.MessageSources.Add(source);
                await ctx.SaveChangesAsync();
            }

            _messageSourceId = source.Id;

            _logger.LogInformation($"Using message source: {source.Name} ({source.Id})");
        }

        await _client.ConnectAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DisconnectAsync();
    }

    private async ValueTask ConnectedAsync()
    {
        await _client.LoginAnonAsync();

        string[] channelLogins;

        using (var scope = _scopeFactory.CreateScope())
        {
            var ctx = scope.Get<TwitchLoggerDbContext>();

            channelLogins = await ctx.ChatLogs
                .AsNoTracking()
                .Select(x => x.Channel.Login)
                .ToArrayAsync();
        }

        var tasks = new List<Task>();
        foreach (var channelLogin in channelLogins)
            tasks.Add(_client.SendAsync(new IrcMessage { Command = IrcCommand.JOIN, Content = new($"#{channelLogin}") }).AsTask());

        await Task.WhenAll(tasks);

        _logger.LogInformation($"Joined {channelLogins.Length} channels: {string.Join(", ", channelLogins)}");
    }

    private async ValueTask IrcMessageReceivedAsync(IrcMessage message)
    {
        if (message.Command != IrcCommand.PRIVMSG) return;

        var channelLogin = message.Arg![1..];
        var channelId = message.Tags!["room-id"];
        var userLogin = message.Prefix!.Name;
        var userId = message.Tags!["user-id"];
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(message.Tags["tmi-sent-ts"]));

        using var scope = _scopeFactory.CreateScope();
        var ctx = scope.Get<TwitchLoggerDbContext>();

        await ctx.TryUpdateUserAsync(channelId, channelLogin, timestamp, _cache, _cacheEntryOptions);
        await ctx.TryUpdateUserAsync(userId, userLogin, timestamp, _cache, _cacheEntryOptions);

        var messageEntity = new Data.Models.Twitch.Message
        {
            ReceivedAt = timestamp,
            SourceId = _messageSourceId,
            ChannelId = channelId,
            AuthorId = userId,
            AuthorLogin = userLogin,
            Content = message.Content!.Text
        };

        ctx.Messages.Add(messageEntity);
        await ctx.SaveChangesAsync();
    }
}
