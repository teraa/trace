using Microsoft.AspNetCore.Mvc;
using Index = Trace.Api.Twitch.Users.Actions.Index;

namespace Trace.Api.Twitch.Users;

[ApiController]
[Route("api/twitch/[controller]")]
// TODO: re-enable
// [Authorize(AuthenticationSchemes = AppAuthScheme.Bearer)]
public class UsersController(
    Index.Handler indexHandler
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] Index.Query query, CancellationToken cancellationToken)
        => await indexHandler.HandleAsync(query, cancellationToken);
}
