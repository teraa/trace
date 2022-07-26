using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitchLogger.Data;

namespace TwitchLogger.Api.Features.Messages.Actions;

public static class Index
{
    public record Query(string ChannelId) : IRequest<IActionResult>;

    [PublicAPI]
    public record Result(
        long Id,
        DateTimeOffset Timestamp,
        string AuthorId,
        string AuthorLogin,
        string Content);

    [UsedImplicitly]
    public class Handler : IRequestHandler<Query, IActionResult>
    {
        private readonly TwitchLoggerDbContext _ctx;

        public Handler(TwitchLoggerDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await _ctx.TmiMessages
                .Where(x => x.ChannelId == request.ChannelId)
                .OrderByDescending(x => x.Timestamp)
                .Select(x => new Result(
                    x.Id,
                    x.Timestamp,
                    x.AuthorId,
                    x.AuthorLogin,
                    x.Content))
                .ToListAsync(cancellationToken);

            return new OkObjectResult(results);
        }
    }
}
