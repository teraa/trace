using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Index = Trace.Api.Twitch.Channels.Actions.Index;

namespace Trace.Api.Twitch.Channels;

[ApiController]
[Route("api/twitch/[controller]")]
[Authorize]
public class ChannelsController(
    Index.Handler indexHandler
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
        => await indexHandler.HandleAsync(new Index.Query(), cancellationToken);
}
