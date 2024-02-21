using Extensions.Hosting.AsyncInitialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Trace.Data;
using Trace.Data.Models.Tmi;
using Trace.Options;

namespace Trace.Tmi;

[UsedImplicitly]
public class SourceInitializer : IAsyncInitializer
{
    private readonly AppDbContext _ctx;
    private readonly IOptionsMonitor<TmiOptions> _options;
    private readonly SourceProvider _sourceProvider;

    public SourceInitializer(AppDbContext ctx, IOptionsMonitor<TmiOptions> options, SourceProvider sourceProvider)
    {
        _ctx = ctx;
        _options = options;
        _sourceProvider = sourceProvider;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var sourceName = _options.CurrentValue.MessageSourceName;

        var source = await _ctx.TmiSources
            .AsNoTracking()
            .Where(x => x.Name == sourceName)
            .FirstOrDefaultAsync(cancellationToken);

        if (source is null)
        {
            source = new Source
            {
                Name = sourceName,
            };

            _ctx.TmiSources.Add(source);
            await _ctx.SaveChangesAsync(cancellationToken);
        }

        _sourceProvider.SourceId = source.Id;
    }
}
