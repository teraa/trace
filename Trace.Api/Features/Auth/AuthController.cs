using MediatR;
using Microsoft.AspNetCore.Mvc;
using Trace.Api.Features.Auth.Actions;

namespace DefaultNamespace;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> Authorize(CancellationToken cancellationToken)
        => await _sender.Send(new Authorize.Command(), cancellationToken);

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> Token(string code,  string scope, string state, CancellationToken cancellationToken = default)
        => await _sender.Send(new Token.Command(code, scope, state), cancellationToken);
}
