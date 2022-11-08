using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trace.Api.Extensions;
using Trace.Data;

namespace Trace.Api.Features.Twitch.Channels.Actions;

public static class Index
{
    public record Query : IRequest<IActionResult>;

    [PublicAPI]
    public record Result(
        string Id,
        string Login
    );

    [UsedImplicitly]
    public class Handler : IRequestHandler<Query, IActionResult>
    {
        private readonly TraceDbContext _ctx;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(TraceDbContext ctx, IHttpContextAccessor httpContextAccessor)
        {
            _ctx = ctx;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext!.GetUserId();

            var twitchUserId = await _ctx.Users
                .Where(x => x.Id == userId)
                .Select(x => x.TwitchId)
                .FirstAsync(cancellationToken);

            var channelIds = await _ctx.ChannelPermissions
                .Where(x => x.UserId == twitchUserId)
                .Select(x => x.ChannelId)
                .ToListAsync(cancellationToken);

            var results = await _ctx.TwitchUsers
                .Where(x => channelIds.Contains(x.Id))
                .OrderByDescending(x => x.LastSeen)
                .Select(x => new Result(x.Id, x.Login))
                .ToListAsync(cancellationToken);

            return new OkObjectResult(results);
        }
    }
}
