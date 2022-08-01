using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitchLogger.Data;

namespace TwitchLogger.Api.Features.Twitch.Users.Actions;

public static class Index
{
    public record Query(
        string? Id,
        string? Login
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
        string Login);

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
            var query = _ctx.TmiMessages
                .AsQueryable();

            if (request.Id is { })
                query = query.Where(x => x.AuthorId == request.Id);

            if (request.Login is { })
                query = query.Where(x => x.AuthorLogin == request.Login);

            var results = await query
                .Select(x => new Result(x.AuthorId, x.AuthorLogin))
                .Distinct()
                .ToListAsync(cancellationToken);

            return new OkObjectResult(results);
        }
    }
}
