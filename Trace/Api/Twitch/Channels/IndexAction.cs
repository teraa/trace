using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Trace.Api.Auth;
using Trace.Data;

namespace Trace.Api.Twitch.Channels;

[Handler]
public static partial class IndexAction
{
    public record Query;

    [PublicAPI]
    public record Result(
        string Id,
        string Login
    );

    private static async ValueTask<IResult> HandleAsync(
        Query _,
        AppDbContext ctx,
        IUserAccessor userAccessor,
        CancellationToken cancellationToken)
    {
        var channelIds = userAccessor.User.Claims
            .Where(x => string.Equals(x.Type, AppClaimTypes.ChannelRead, StringComparison.Ordinal))
            .Select(x => x.Value)
            .ToList();

        var channels = await ctx.TwitchUsers
            .AsNoTracking()
            .Where(x => channelIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var results = channels
            .GroupBy(x => x.Id)
            .Select(x => x.MaxBy(static x => x.LastSeen)!)
            .Select(x => new Result(x.Id, x.Login))
            .ToList();

        return Results.Ok(results);
    }
}
