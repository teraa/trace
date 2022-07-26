using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitchLogger.Data;
using TwitchLogger.Data.Models.Tmi;

namespace TwitchLogger.Api.Features.Messages.Actions;

public static class Index
{
    public record Query(
        string ChannelId,
        int Limit,
        long? Before
    ) : IRequest<IActionResult>;

    // TODO: QueryValidator

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
            IQueryable<Message> query = _ctx.TmiMessages
                .Where(x => x.ChannelId == request.ChannelId)
                .OrderByDescending(x => x.Timestamp)
                .ThenBy(x => x.Id);

            if (request.Before is not null)
            {
                var beforeTimestamp = await query
                    .Where(x => x.Id == request.Before)
                    .Select(x => (DateTimeOffset?)x.Timestamp)
                    .FirstOrDefaultAsync(cancellationToken);

                if (beforeTimestamp is null)
                    return new BadRequestResult(); // TODO

                int offset = await query
                    .Where(x => x.Timestamp == beforeTimestamp)
                    .Where(x => x.Id <= request.Before)
                    .CountAsync(cancellationToken);

                query = query
                    .Where(x => x.Timestamp <= beforeTimestamp)
                    .Skip(offset);
            }

            var results = await query
                .Take(request.Limit)
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
