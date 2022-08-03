using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Index = Trace.Api.Features.Twitch.Messages.Actions.Index;

namespace Trace.Api.Features.Twitch.Messages;

[ApiController]
[Route("twitch/[controller]")]
[Authorize(AuthenticationSchemes = AppAuthScheme.Bearer)]
public class MessagesController : ControllerBase
{
    private readonly ISender _sender;

    public MessagesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string channelId, int limit, long? before, string? authorId, string? authorLogin, CancellationToken cancellationToken)
        => await _sender.Send(new Index.Query(channelId, limit, before, authorId, authorLogin), cancellationToken);
}
