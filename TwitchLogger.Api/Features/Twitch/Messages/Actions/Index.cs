using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trace.Data;
using Trace.Data.Models.Tmi;

namespace Trace.Api.Features.Twitch.Messages.Actions;

public static class Index
{
    public record Query(
        string ChannelId,
        int Limit,
        long? Before,
        string? AuthorId,
        string? AuthorLogin
    ) : IRequest<IActionResult>;

    [UsedImplicitly]
    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
            RuleFor(x => x.Limit).InclusiveBetween(1, 100);
        }
    }

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
        private readonly TraceDbContext _ctx;

        public Handler(TraceDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _ctx.TmiMessages
                .Where(x => x.ChannelId == request.ChannelId)
                .OrderByDescending(x => x.Timestamp)
                .ThenBy(x => x.Id)
                .AsQueryable();

            if (request.AuthorId is { })
                query = query.Where(x => x.AuthorId == request.AuthorId);

            if (request.AuthorLogin is { })
                query = query.Where(x => x.AuthorLogin == request.AuthorLogin);

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
