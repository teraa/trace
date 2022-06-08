using Extensions.Hosting.AsyncInitialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TwitchLogger.Data;
using TwitchLogger.Data.Models.Tmi;
using TwitchLogger.Options;

namespace TwitchLogger.Initializers;

[UsedImplicitly]
public class TmiSourceInitializer : IAsyncInitializer
{
    private readonly IMemoryCache _cache;
    private readonly TwitchLoggerDbContext _ctx;
    private readonly ChatOptions _options;
    private readonly ILogger<TmiSourceInitializer> _logger;

    public TmiSourceInitializer(
        IMemoryCache cache,
        TwitchLoggerDbContext ctx,
        IOptions<ChatOptions> options,
        ILogger<TmiSourceInitializer> logger)
    {
        _cache = cache;
        _ctx = ctx;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InitializeAsync()
    {
        var source = await _ctx.TmiSources
            .AsNoTracking()
            .Where(x => x.Name == _options.MessageSourceName)
            .FirstOrDefaultAsync();

        if (source is null)
        {
            source = new Source
            {
                Name = _options.MessageSourceName,
            };

            _ctx.TmiSources.Add(source);
            await _ctx.SaveChangesAsync();
        }

        _cache.Set("source_id", source.Id);
        _logger.LogInformation("Using chat source: {SourceName}", source.Name);
    }
}
