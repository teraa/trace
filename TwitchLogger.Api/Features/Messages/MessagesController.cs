using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Index = TwitchLogger.Api.Features.Messages.Actions.Index;

namespace TwitchLogger.Api.Features.Messages;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<Index.Query> _validator;

    public MessagesController(IMediator mediator, IValidator<Index.Query> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string channelId, int limit = 50, long? before = null, CancellationToken cancellationToken = default)
    {
        var query = new Index.Query(channelId, limit, before);
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);

        if (validationResult.IsValid)
            return await _mediator.Send(query, cancellationToken);

        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return ValidationProblem();
    }
}
