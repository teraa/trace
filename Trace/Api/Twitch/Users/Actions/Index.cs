using System.Text.RegularExpressions;
using FluentValidation;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trace.Data;
using Trace.Data.Models.Twitch;
using Results = Teraa.Extensions.AspNetCore.Results;

namespace Trace.Api.Twitch.Users.Actions;

[Handler]
public static partial class Index
{
    public record Query(
        IReadOnlyList<string>? Ids = null,
        IReadOnlyList<string>? Logins = null,
        string? LoginPattern = null,
        int PatternLimit = 10,
        bool Recursive = false
    );

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
                .Must(x => x is {Ids: not null} or {Logins: not null} or {LoginPattern: not null})
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

    private static async ValueTask<IActionResult> HandleAsync(
        Query request,
        AppDbContext ctx,
        CancellationToken cancellationToken)
    {
        var predicate = PredicateBuilder.New<User>();

        if (request.Ids is not null)
            predicate = predicate.Or(x => request.Ids.Contains(x.Id));

        if (request.Logins is not null)
        {
            var logins = request.Logins.Select(x => x.ToLowerInvariant());

            if (request.Recursive)
            {
                var userIds = await ctx.TwitchUsers
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

        if (request.LoginPattern is not null)
        {
            ctx.Database.SetCommandTimeout(5);

            var patternQuery = ctx.TwitchUsers
                // ReSharper disable once EntityFramework.UnsupportedServerSideFunctionCall
                .Where(x => Regex.IsMatch(x.Login, request.LoginPattern,
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                .OrderBy(x => x.Login.Length);

            if (request.Recursive)
            {
                List<string> userIds;
                try
                {
                    userIds = await patternQuery
                        .Select(x => x.Id)
                        .Take(request.PatternLimit)
                        .Distinct()
                        .ToListAsync(cancellationToken);
                }
                catch (Npgsql.PostgresException ex) when (ex.SqlState == "2201B")
                {
                    return Results.BadRequestDetails("Invalid pattern.");
                }

                predicate = predicate.Or(x => userIds.Contains(x.Id));
            }
            else
            {
                List<string> userLogins;
                try
                {
                    userLogins = await patternQuery
                        .Select(x => x.Login)
                        .Take(request.PatternLimit)
                        .Distinct()
                        .ToListAsync(cancellationToken);
                }
                catch (Npgsql.PostgresException ex) when (ex.SqlState == "2201B")
                {
                    return Results.BadRequestDetails("Invalid pattern.");
                }

                predicate = predicate.Or(x => userLogins.Contains(x.Login));
            }
        }

        var results = await ctx.TwitchUsers
            .Where(predicate)
            .Select(x => new Result(x.Id, x.Login, x.FirstSeen, x.LastSeen))
            .ToListAsync(cancellationToken);

        return new OkObjectResult(results);
    }
}
