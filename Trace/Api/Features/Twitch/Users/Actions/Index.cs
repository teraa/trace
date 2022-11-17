using FluentValidation;
using JetBrains.Annotations;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trace.Data;
using Trace.Data.Models.Twitch;

namespace Trace.Api.Features.Twitch.Users.Actions;

public static class Index
{
    public record Query(
        IReadOnlyList<string>? Ids,
        IReadOnlyList<string>? Logins,
        bool Recursive = false
    ) : IRequest<IActionResult>;

    [UsedImplicitly]
    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleForEach(x => x.Ids).NotEmpty();
            RuleForEach(x => x.Logins).NotEmpty();

            RuleFor(x => x)
                .Must(x => x is {Ids: { }} or {Logins: { }})
                .WithMessage($"Must include at least one of {nameof(Query.Ids)} and {nameof(Query.Logins)}");
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
            var predicate = PredicateBuilder.New<User>();

            if (request.Ids is { })
                predicate = predicate.Or(x => request.Ids.Contains(x.Id));

            if (request.Logins is { })
            {
                var logins = request.Logins.Select(x => x.ToLowerInvariant());

                if (request.Recursive)
                {
                    var userIds = await _ctx.TwitchUsers
                        .Where(x => logins.Contains(x.Login))
                        .Select(x => x.Id)
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    predicate = predicate.Or(x => userIds.Contains(x.Id));
                }
                else
                {
                    predicate = predicate.Or(x => logins.Contains(x.Login));
                }
            }

            var results = await _ctx.TwitchUsers
                .Where(predicate)
                .Select(x => new Result(x.Id, x.Login, x.FirstSeen, x.LastSeen))
                .ToListAsync(cancellationToken);

            return new OkObjectResult(results);
        }
    }
}
