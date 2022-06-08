using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TwitchLogger.Data;
using TwitchLogger.Data.Models.Tmi;

namespace TwitchLogger.Tmi;

public class SourceCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;

    public SourceCache(IServiceScopeFactory scopeFactory, IMemoryCache cache)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
    }

    public async Task<short> GetOrLoadSourceIdAsync(string sourceName, CancellationToken cancellationToken)
    {
        var lazy = _cache.GetOrCreate(new SourceKey(sourceName), entry =>
        {
            return new Lazy<Task<short>>(async () =>
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var ctx = scope.ServiceProvider.GetRequiredService<TwitchLoggerDbContext>();

                var source = await ctx.TmiSources
                    .AsNoTracking()
                    .Where(x => x.Name == sourceName)
                    .FirstOrDefaultAsync(cancellationToken);

                if (source is not null)
                    return source.Id;

                source = new Source
                {
                    Name = sourceName,
                };

                ctx.TmiSources.Add(source);
                await ctx.SaveChangesAsync(cancellationToken);

                return source.Id;
            });
        });

        return await lazy.Value;
    }

    private record SourceKey(string SourceName);
}
