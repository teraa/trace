using System.Text.RegularExpressions;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;
using Trace.Data;

namespace Trace.Common;

[Handler]
public sealed partial class UpdateUsers(
    AppDbContext ctx,
    ILogger<UpdateUsers> logger)
{
    public sealed record Command(
        IReadOnlyList<Command.User> Users,
        DateTimeOffset Timestamp)
    {
        public sealed record User(
            string Id,
            string Login
        );
    }

    private static readonly Regex s_loginRegex = new("^[a-z0-9_]+$", RegexOptions.Compiled);

    private async ValueTask HandleAsync(
        Command request,
        CancellationToken cancellationToken)
    {
        var users = new Dictionary<string, string>(request.Users.Count);

        foreach (var user in request.Users)
        {
            if (!s_loginRegex.IsMatch(user.Login))
            {
                logger.LogWarning("Skipped updating user with non-login value as a name: {UserName} ({UserId})",
                    user.Login, user.Id);

                continue;
            }

            if (!users.TryAdd(user.Id, user.Login))
            {
                logger.LogDebug("Skipped duplicate user in request: {UserName} ({UserId})", user.Login, user.Id);
            }
        }

        var entities = await ctx.TwitchUsers
            .Where(x => users.Keys.Contains(x.Id))
            .GroupBy(x => x.Id)
            .Select(x => x.OrderByDescending(y => y.LastSeen).First())
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        foreach (var (userId, userLogin) in users)
        {
            if (entities.TryGetValue(userId, out var entity)
                && string.Equals(entity.Login, userLogin, StringComparison.Ordinal))
            {
                entity.LastSeen = request.Timestamp;
            }
            else
            {
                entity = new Data.Models.Twitch.User
                {
                    Id = userId,
                    Login = userLogin,
                    FirstSeen = request.Timestamp,
                    LastSeen = request.Timestamp,
                };

                ctx.TwitchUsers.Add(entity);
            }
        }

        await ctx.SaveChangesAsync(cancellationToken);
    }
}
