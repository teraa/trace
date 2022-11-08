using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trace.Data;

namespace Trace.Api.Features.Twitch.Users.Actions;

public static class Index
{
    public record Query(
        string? Id,
        string? Login,
        bool? Recursive
    ) : IRequest<IActionResult>;

    [UsedImplicitly]
    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(x => x)
                .Must(x => x.Id is { } || x.Login is { })
                .WithMessage($"Must include at least one of {nameof(Query.Id)} and {nameof(Query.Login)}");
        }
    }

    [PublicAPI]
    public record Result(
        string Id,
        string Login,
        DateTimeOffset FirstSeen,
        DateTimeOffset LastSeen);

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
            var query = _ctx.TwitchUsers
                .AsQueryable();

            if (request.Id is { })
                query = query.Where(x => x.Id == request.Id);

            if (request.Login is { })
            {
                if (request.Recursive is true)
                {
                    var userIds = await _ctx.TwitchUsers
                        .Where(x => x.Login == request.Login)
                        .Select(x => x.Id)
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    query = query.Where(x => userIds.Contains(x.Id));
                }
                else
                {
                    query = query.Where(x => x.Login == request.Login);
                }
            }

            var results = await query
                .Select(x => new Result(x.Id, x.Login, x.FirstSeen, x.LastSeen))
                .ToListAsync(cancellationToken);

            return new OkObjectResult(results);
        }
    }
}
