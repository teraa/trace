using MediatR;
using Microsoft.AspNetCore.Mvc;
using Index = Trace.Api.Twitch.Users.Actions.Index;

namespace Trace.Api.Twitch.Users;

[ApiController]
[Route("api/twitch/[controller]")]
// TODO: re-enable
// [Authorize(AuthenticationSchemes = AppAuthScheme.Bearer)]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] Index.Query query, CancellationToken cancellationToken)
        => await _sender.Send(query, cancellationToken);
}
