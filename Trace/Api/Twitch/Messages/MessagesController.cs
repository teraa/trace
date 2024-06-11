using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Index = Trace.Api.Twitch.Messages.Actions.Index;

namespace Trace.Api.Twitch.Messages;

[ApiController]
[Route("api/twitch/[controller]")]
[Authorize]
public class MessagesController(
    Index.Handler indexHandler
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] Index.Query query, CancellationToken cancellationToken)
        => await indexHandler.HandleAsync(query, cancellationToken);
}
