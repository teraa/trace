using FluentValidation;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trace.Api.Auth;
using Trace.Data;
using Results = Teraa.Extensions.AspNetCore.Results;

namespace Trace.Api.Twitch.Messages.Actions;

[Handler]
public static partial class Index
{
    public record Query(
        string ChannelId,
        int Limit,
        long? Before,
        string? AuthorId,
        DateTimeOffset? BeforeTimestamp
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


    private static async ValueTask<IActionResult> HandleAsync(
        Query request,
        AppDbContext ctx,
        IUserAccessor userAccessor,
        IAuthorizationService authorizationService,
        CancellationToken cancellationToken)
    {
        var authzResult =
            await authorizationService.AuthorizeAsync(userAccessor.User, request.ChannelId, AppAuthzPolicies.Channel);
        if (!authzResult.Succeeded)
            return new ForbidResult();

        var query = ctx.TmiMessages
            .Where(x => x.ChannelId == request.ChannelId)
            .OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Id)
            .AsQueryable();

        if (request.AuthorId is { })
            query = query.Where(x => x.AuthorId == request.AuthorId);

        if (request.Before is { })
        {
            var beforeTimestamp = await query
                .Where(x => x.Id == request.Before)
                .Select(x => (DateTimeOffset?) x.Timestamp)
                .FirstOrDefaultAsync(cancellationToken);

            if (beforeTimestamp is null)
                return Results.BadRequestDetails("Invalid before ID.");

            int offset = await query
                .Where(x => x.Timestamp == beforeTimestamp)
                .Where(x => x.Id <= request.Before)
                .CountAsync(cancellationToken);

            query = query
                .Where(x => x.Timestamp <= beforeTimestamp)
                .Skip(offset);
        }

        if (request.BeforeTimestamp is { })
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

        return new OkObjectResult(results);
    }
}
