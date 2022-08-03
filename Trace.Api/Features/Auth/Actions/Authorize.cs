using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Trace.Api.Options;

namespace Trace.Api.Features.Auth.Actions;

public static class Authorize
{
    public record Command : IRequest<IActionResult>;

    [UsedImplicitly]
    public class Handler : IRequestHandler<Command, IActionResult>
    {
        private readonly IOptionsMonitor<TwitchOptions> _options;
        private readonly IMemoryCache _cache;

        public Handler(IOptionsMonitor<TwitchOptions> options, IMemoryCache cache)
        {
            _options = options;
            _cache = cache;
        }

        public Task<IActionResult> Handle(Command request, CancellationToken cancellationToken)
        {
            string state = _cache.CreateState(_options.CurrentValue.StateLifetime);

            var url = new UriBuilder(_options.CurrentValue.AuthorizationEndpoint)
            {
                Query = new QueryBuilder(new Dictionary<string, string>
                {
                    ["client_id"] = _options.CurrentValue.ClientId,
                    ["redirect_uri"] = _options.CurrentValue.RedirectUri.ToString(),
                    ["response_type"] = "code",
                    ["scope"] = _options.CurrentValue.Scope,
                    ["state"] = state,
                    // ["force_verify"] = "true",
                }).ToString()
            }.ToString();

            var result = new RedirectResult(url);

            return Task.FromResult<IActionResult>(result);
        }
    }
}
