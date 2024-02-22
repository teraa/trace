using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trace.Data;

namespace Trace.Api.Twitch.Channels.Actions;

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
        private readonly AppDbContext _ctx;
        private readonly IUserAccessor _userAccessor;

        public Handler(AppDbContext ctx, IUserAccessor userAccessor)
        {
            _ctx = ctx;
            _userAccessor = userAccessor;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
        {
            var channelIds = _userAccessor.User.Claims
                .Where(x => string.Equals(x.Type, AppClaimTypes.ChannelRead))
                .Select(x => x.Value)
                .ToList();

            var channels = await _ctx.TwitchUsers
                .AsNoTracking()
                .Where(x => channelIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var results = channels
                .GroupBy(x => x.Id)
                .Select(x => x.MaxBy(static x => x.LastSeen)!)
                .Select(x => new Result(x.Id, x.Login))
                .ToList();

            return new OkObjectResult(results);
        }
    }
}
