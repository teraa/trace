using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Trace.Api.Options;
using Trace.Data;
using Trace.Data.Models;

namespace Trace.Api.Features.Auth.Actions;

public static class CreateToken
{
    public record Command(
        string Code,
        string Scope,
        string State
    ) : IRequest<IActionResult>;

    [UsedImplicitly]
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator(IOptionsMonitor<TwitchOptions> options)
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Scope)
                .Equal(options.CurrentValue.Scope)
                .When(x => x.Scope is {Length: > 0} || options.CurrentValue.Scope is {Length: > 0});
            RuleFor(x => x.State).NotEmpty();
        }
    }

    [PublicAPI]
    public record Result(
        string Token,
        int ExpiresIn,
        Guid RefreshToken);

    [UsedImplicitly]
    public class Handler : IRequestHandler<Command, IActionResult>
    {
        private readonly IMemoryCache _cache;
        private readonly IOptionsMonitor<TwitchOptions> _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenService _tokenService;
        private readonly TraceDbContext _ctx;

        public Handler(
            IMemoryCache cache,
            IOptionsMonitor<TwitchOptions> options,
            IHttpClientFactory httpClientFactory,
            TokenService tokenService,
            TraceDbContext ctx)
        {
            _cache = cache;
            _options = options;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _ctx = ctx;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken)
        {
            if (!_cache.InvalidateState(request.State))
                return Results.BadRequestDetails("Invalid state.");

            var client = _httpClientFactory.CreateClient();

            TokenResponse tokenResponse;
            {
                using var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = _options.CurrentValue.TokenEndpoint,
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["client_id"] = _options.CurrentValue.ClientId,
                        ["client_secret"] = _options.CurrentValue.ClientSecret,
                        ["grant_type"] = "authorization_code",
                        ["code"] = request.Code,
                        ["redirect_uri"] = _options.CurrentValue.RedirectUri.ToString(),
                    }),
                };

                using var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                    return Results.BadRequestDetails("Authorization failed.");

                var data = await httpResponse.Content
                    .ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);

                Debug.Assert(data is not null);
                tokenResponse = data;
            }

            ValidateResponse validateResponse;
            {
                using var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = _options.CurrentValue.ValidateEndpoint,
                    Headers =
                    {
                        Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken),
                    },
                };

                using var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                    return Results.BadRequestDetails("Token validation failed.");

                var data = await httpResponse.Content
                    .ReadFromJsonAsync<ValidateResponse>(cancellationToken: cancellationToken);

                Debug.Assert(data is not null);
                validateResponse = data;
            }

            var userEntity = await _ctx.Users
                .Where(x => x.TwitchId == validateResponse.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEntity is null)
            {
                userEntity = new User
                {
                    Id = Guid.NewGuid(),
                    TwitchId = validateResponse.UserId,
                    TwitchLogin = validateResponse.Login,
                    IsVerified = false,
                };

                _ctx.Users.Add(userEntity);
            }
            else
            {
                userEntity.TwitchLogin = validateResponse.Login;
            }

            if (!userEntity.IsVerified)
            {
                await _ctx.SaveChangesAsync(cancellationToken);
                return new ForbidResult();
            }

            var now = DateTimeOffset.UtcNow;
            var tokenData = _tokenService.CreateToken(now, userEntity.Id);

            var refreshTokenEntity = new Data.Models.RefreshToken
            {
                Id = tokenData.RefreshToken.Value,
                User = userEntity,
                IssuedAt = now,
                ExpiresAt = now + tokenData.RefreshToken.ExpiresIn,
            };

            _ctx.RefreshTokens.Add(refreshTokenEntity);
            await _ctx.SaveChangesAsync(cancellationToken);

            var result = new Result(tokenData.Token.Value, (int) tokenData.Token.ExpiresIn.TotalSeconds, tokenData.RefreshToken.Value);
            return new OkObjectResult(result);
        }

        [UsedImplicitly]
        private record TokenResponse(
            [property: JsonPropertyName("access_token")]
            string AccessToken,
            [property: JsonPropertyName("expires_in")]
            int ExpiresIn,
            [property: JsonPropertyName("refresh_token")]
            string RefreshToken,
            [property: JsonPropertyName("token_type")]
            string TokenType);

        [UsedImplicitly]
        private record ValidateResponse(
            [property: JsonPropertyName("client_id")]
            string ClientId,
            [property: JsonPropertyName("login")]
            string Login,
            [property: JsonPropertyName("scopes")]
            string[] Scopes,
            [property: JsonPropertyName("user_id")]
            string UserId,
            [property: JsonPropertyName("expires_in")]
            int ExpiresIn);
    }
}
