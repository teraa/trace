using System.Text.RegularExpressions;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;
using Trace.Data;
using Trace.Data.Models.Twitch;

namespace Trace.Features.Users;

[Handler]
public static partial class UpdateUser
{
    public sealed record Command(
        string Id,
        string Login,
        DateTimeOffset Timestamp
    );

    private static readonly Regex s_loginRegex = new("^[a-z0-9_]+$", RegexOptions.Compiled);

    private static async ValueTask HandleAsync(
        Command request,
        AppDbContext ctx,
        ILogger<Command> logger,
        CancellationToken cancellationToken)
    {
        if (!s_loginRegex.IsMatch(request.Login))
        {
            logger.LogWarning("Skipped updating user with non-login value as a name: {UserName} ({UserId})",
                request.Login, request.Id);
            return;
        }

        var entity = await ctx.TwitchUsers
            .Where(x => x.Id == request.Id)
            .OrderByDescending(x => x.LastSeen)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null || !string.Equals(entity.Login, request.Login, StringComparison.Ordinal))
        {
            entity = new User
            {
                Id = request.Id,
                Login = request.Login,
                FirstSeen = request.Timestamp,
                LastSeen = request.Timestamp,
            };

            ctx.TwitchUsers.Add(entity);
        }
        else
        {
            entity.LastSeen = request.Timestamp;
        }

        await ctx.SaveChangesAsync(cancellationToken);
    }
}
