using MediatR;
using Microsoft.AspNetCore.Mvc;
using Index = TwitchLogger.Api.Features.Twitch.Users.Actions.Index;

namespace TwitchLogger.Api.Features.Twitch.Users;

[ApiController]
[Route("twitch/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? id, string? login, CancellationToken cancellationToken)
        => await _sender.Send(new Index.Query(id, login), cancellationToken);
}
