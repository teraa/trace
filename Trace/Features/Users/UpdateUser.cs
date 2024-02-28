using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Trace.Data;
using Trace.Data.Models.Twitch;

namespace Trace.Features.Users;

public static class UpdateUser
{
    public sealed record Command(
        string Id,
        string Login,
        DateTimeOffset Timestamp
    ) : IRequest;

    [UsedImplicitly]
    public sealed class Handler : IRequestHandler<Command>
    {
        private static readonly Regex s_loginRegex = new Regex("^[a-z0-9_]+$", RegexOptions.Compiled);

        private readonly AppDbContext _ctx;
        private readonly ILogger<Handler> _logger;

        public Handler(AppDbContext ctx, ILogger<Handler> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            if (!s_loginRegex.IsMatch(request.Login))
            {
                _logger.LogWarning("Skipped updating user with non-login value as a name: {UserName} ({UserId})", request.Login, request.Id);
                return;
            }

            var entity = await _ctx.TwitchUsers
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

                _ctx.TwitchUsers.Add(entity);
            }
            else
            {
                entity.LastSeen = request.Timestamp;
            }

            await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
