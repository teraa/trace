using FluentValidation;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Trace.Api.Auth;
using Trace.Data;

namespace Trace.Api.Twitch.Messages;

[Handler]
public static partial class IndexAction
{
    public record Query(
        string ChannelId,
        int Limit = 100,
        long? Before = null,
        string? AuthorId = null,
        DateTimeOffset? BeforeTimestamp = null
    );

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


    private static async ValueTask<IResult> HandleAsync(
        Query request,
        AppDbContext ctx,
        IUserAccessor userAccessor,
        IAuthorizationService authorizationService,
        CancellationToken cancellationToken)
    {
        var authzResult =
            await authorizationService.AuthorizeAsync(userAccessor.User, request.ChannelId, AppAuthzPolicies.Channel);
        if (!authzResult.Succeeded)
            return Results.Forbid();

        var query = ctx.TmiMessages
            .Where(x => x.ChannelId == request.ChannelId)
            .OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Id)
            .AsQueryable();

        if (request.AuthorId is not null)
            query = query.Where(x => x.AuthorId == request.AuthorId);

        if (request.Before is not null)
        {
            var beforeTimestamp = await query
                .Where(x => x.Id == request.Before)
                .Select(x => (DateTimeOffset?) x.Timestamp)
                .FirstOrDefaultAsync(cancellationToken);

            if (beforeTimestamp is null)
                return Results.Problem("Invalid before ID.", statusCode: StatusCodes.Status400BadRequest);

            int offset = await query
                .Where(x => x.Timestamp == beforeTimestamp)
                .Where(x => x.Id <= request.Before)
                .CountAsync(cancellationToken);

            query = query
                .Where(x => x.Timestamp <= beforeTimestamp)
                .Skip(offset);
        }

        if (request.BeforeTimestamp is not null)
        {
            var beforeTimestamp = request.BeforeTimestamp.Value.ToUniversalTime();
            query = query.Where(x => x.Timestamp <= beforeTimestamp);
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

        return Results.Ok(results);
    }
}
