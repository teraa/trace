using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trace.Data;

namespace Trace.Api.Features.Twitch.Channels.Actions;

public static class Index
{
    public record Query : IRequest<IActionResult>;

    [PublicAPI]
    public record Result(
        string Id,
        string Login
    );

    [UsedImplicitly]
    public class Handler : IRequestHandler<Query, IActionResult>
    {
        private readonly TraceDbContext _ctx;

        public Handler(TraceDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await _ctx.TmiConfigs
                .OrderBy(x => x.ChannelLogin)
                .Select(x => new Result(x.ChannelId, x.ChannelLogin))
                .ToListAsync(cancellationToken);

            return new OkObjectResult(results);
        }
    }
}
