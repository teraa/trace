using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Index = Trace.Api.Twitch.Channels.Actions.Index;

namespace Trace.Api.Twitch.Channels;

[ApiController]
[Route("twitch/[controller]")]
[Authorize]
public class ChannelsController : ControllerBase
{
    private readonly ISender _sender;

    public ChannelsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
        => await _sender.Send(new Index.Query(), cancellationToken);
}
