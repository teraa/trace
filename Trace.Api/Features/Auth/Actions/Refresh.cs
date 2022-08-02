using System.Security.Claims;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trace.Data;
using Trace.Data.Models;

namespace Trace.Api.Features.Auth.Actions;

public static class Refresh
{
    public record Command(
        Guid RefreshToken
    ) : IRequest<IActionResult>;

    [UsedImplicitly]
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty();
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TraceDbContext _ctx;
        private readonly TokenService _tokenService;

        public Handler(IHttpContextAccessor httpContextAccessor, TraceDbContext ctx, TokenService tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _ctx = ctx;
            _tokenService = tokenService;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var userIdValue = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = new Guid(userIdValue);
            var now = DateTimeOffset.UtcNow;

            var refreshTokenEntity = await _ctx.RefreshTokens
                .Where(x => x.Id == request.RefreshToken)
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (refreshTokenEntity is null)
                return Results.BadRequestDetails("Invalid refresh token.");

            _ctx.RefreshTokens.Remove(refreshTokenEntity);

            if (!_tokenService.IsValid(now, refreshTokenEntity.ExpiresAt))
            {
                await _ctx.SaveChangesAsync(cancellationToken);
                return Results.BadRequestDetails("Expired refresh token.");
            }

            var tokenData = _tokenService.CreateToken(now, refreshTokenEntity.UserId);

            refreshTokenEntity = new RefreshToken
            {
                Id = tokenData.RefreshToken,
                UserId = refreshTokenEntity.UserId,
                IssuedAt = now,
                ExpiresAt = now + tokenData.RefreshTokenExpiresIn,
            };

            _ctx.RefreshTokens.Add(refreshTokenEntity);
            await _ctx.SaveChangesAsync(cancellationToken);

            var result = new Result(tokenData.Token, (int) tokenData.TokenExpiresIn.TotalSeconds, tokenData.RefreshToken);
            return new OkObjectResult(result);
        }
    }
}
