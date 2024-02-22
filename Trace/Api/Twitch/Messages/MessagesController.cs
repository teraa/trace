using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Index = Trace.Api.Twitch.Messages.Actions.Index;

namespace Trace.Api.Twitch.Messages;

[ApiController]
[Route("twitch/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly ISender _sender;

    public MessagesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] Index.Query query, CancellationToken cancellationToken)
        => await _sender.Send(query, cancellationToken);
}
