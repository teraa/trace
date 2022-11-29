using MediatR;
// ReSharper disable once RedundantUsingDirective
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Index = Trace.Api.Features.Twitch.Users.Actions.Index;

namespace Trace.Api.Features.Twitch.Users;

[ApiController]
[Route("twitch/[controller]")]
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
