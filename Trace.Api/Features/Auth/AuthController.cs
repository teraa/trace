using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trace.Api.Features.Auth.Actions;

namespace Trace.Api.Features.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Login(CancellationToken cancellationToken)
        => await _sender.Send(new Redirect.Command(), cancellationToken);

    [HttpGet("[action]")]
    public async Task<IActionResult> Callback([FromQuery] CreateToken.Command command, CancellationToken cancellationToken)
        => await _sender.Send(command, cancellationToken);

    [HttpPost("[action]")]
    [Authorize(AuthenticationSchemes = AppAuthScheme.ExpiredBearer)]
    public async Task<IActionResult> Refresh(RefreshToken.Command command, CancellationToken cancellationToken)
        => await _sender.Send(command, cancellationToken);
}
