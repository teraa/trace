using MediatR;
using Microsoft.AspNetCore.Mvc;
using Index = Trace.Api.Features.Twitch.Channels.Actions.Index;

namespace Trace.Api.Features.Twitch.Channels;

[ApiController]
[Route("twitch/[controller]")]
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
