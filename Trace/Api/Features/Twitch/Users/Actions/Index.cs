using System.Text.RegularExpressions;
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
        string? LoginPattern,
        int PatternLimit = 10,
        bool Recursive = false
    ) : IRequest<IActionResult>;

    [UsedImplicitly]
    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleForEach(x => x.Ids).NotEmpty();
            RuleForEach(x => x.Logins).NotEmpty();
            RuleFor(x => x.LoginPattern).MinimumLength(1);
            RuleFor(x => x.PatternLimit).InclusiveBetween(1, 1000);

            RuleFor(x => x)
                .Must(x => x is {Ids: { }} or {Logins: { }} or {LoginPattern: { }})
                .WithMessage(
                    $"Must include at least one of {nameof(Query.Ids)}, {nameof(Query.Logins)}, or {nameof(Query.LoginPattern)}");
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

            if (request.LoginPattern is { })
            {
                _ctx.Database.SetCommandTimeout(5);

                var patternQuery = _ctx.TwitchUsers
                    .Where(x => Regex.IsMatch(x.Login, request.LoginPattern,
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                    .OrderBy(x => x.Login.Length);

                if (request.Recursive)
                {
                    var userIds = await patternQuery
                        .Select(x => x.Id)
                        .Take(request.PatternLimit)
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    predicate = predicate.Or(x => userIds.Contains(x.Id));
                }
                else
                {
                    var userLogins = await patternQuery
                        .Select(x => x.Login)
                        .Take(request.PatternLimit)
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    predicate = predicate.Or(x => userLogins.Contains(x.Login));
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
