﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Trace.Data;
using Trace.Data.Models.Tmi;

namespace Trace.Tmi;

public class SourceCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;

    public SourceCache(IServiceScopeFactory scopeFactory,
        IMemoryCache cache)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
    }

    public async Task<short> GetOrLoadSourceIdAsync(CancellationToken cancellationToken)
    {
        var lazy = _cache.GetOrCreate(new SourceKey(), entry =>
        {
            return new Lazy<Task<short>>(async () =>
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var ctx = scope.ServiceProvider.GetRequiredService<TraceDbContext>();
                var options = scope.ServiceProvider.GetRequiredService<IOptions<TmiOptions>>();

                var source = await ctx.TmiSources
                    .AsNoTracking()
                    .Where(x => x.Name == options.Value.MessageSourceName)
                    .FirstOrDefaultAsync(cancellationToken);

                if (source is not null)
                    return source.Id;

                source = new Source
                {
                    Name = options.Value.MessageSourceName,
                };

                ctx.TmiSources.Add(source);
                await ctx.SaveChangesAsync(cancellationToken);

                return source.Id;
            });
        });

        return await lazy.Value;
    }

    private record SourceKey;
}