using MediatR;
using Microsoft.AspNetCore.Mvc;
using Index = TwitchLogger.Api.Features.Messages.Actions.Index;

namespace TwitchLogger.Api.Features.Messages;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string channelId, int limit = 50, long? before = null, CancellationToken cancellationToken = default)
        => await _mediator.Send(new Index.Query(channelId, limit, before), cancellationToken);
}
