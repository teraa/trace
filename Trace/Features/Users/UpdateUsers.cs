using System.Text.RegularExpressions;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;
using Trace.Data;

namespace Trace.Features.Users;

[Handler]
public static partial class UpdateUsers
{
    public sealed record Command(
        IReadOnlyList<Command.User> Users,
        DateTimeOffset Timestamp
    )
    {
        public sealed record User(
            string Id,
            string Login
        );
    }

    private static readonly Regex s_loginRegex = new("^[a-z0-9_]+$", RegexOptions.Compiled);

    private static async ValueTask HandleAsync(
        Command request,
        AppDbContext ctx,
        ILogger<Command> logger,
        CancellationToken cancellationToken)
    {
        var users = new Dictionary<string, Command.User>(request.Users.Count);

        foreach (var user in request.Users)
        {
            if (!s_loginRegex.IsMatch(user.Login))
            {
                logger.LogWarning("Skipped updating user with non-login value as a name: {UserName} ({UserId})",
                    user.Login, user.Id);

                continue;
            }

            if (!users.TryAdd(user.Id, user))
            {
                logger.LogDebug("Skipped duplicate user in request: {UserName} ({UserId})", user.Login, user.Id);
            }
        }

        var entities = await ctx.TwitchUsers
            .Where(x => users.Keys.Contains(x.Id))
            .GroupBy(x => x.Id)
            .Select(x => x.MaxBy(y => y.LastSeen)!)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        foreach (var (_, user) in users)
        {
            if (entities.TryGetValue(user.Id, out var entity)
                && string.Equals(entity.Login, user.Login, StringComparison.Ordinal))
            {
                entity.LastSeen = request.Timestamp;
            }
            else
            {
                entity = new Data.Models.Twitch.User
                {
                    Id = user.Id,
                    Login = user.Login,
                    FirstSeen = request.Timestamp,
                    LastSeen = request.Timestamp,
                };

                ctx.TwitchUsers.Add(entity);
            }
        }

        await ctx.SaveChangesAsync(cancellationToken);
    }
}
