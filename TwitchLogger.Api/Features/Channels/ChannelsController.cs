using MediatR;
using Microsoft.AspNetCore.Mvc;
using Index = TwitchLogger.Api.Features.Channels.Actions.Index;

namespace TwitchLogger.Api.Features.Channels;

[ApiController]
[Route("[controller]")]
public class ChannelsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChannelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
        => await _mediator.Send(new Index.Query(), cancellationToken);
}
