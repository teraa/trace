using FluentValidation;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Trace.Options;

#pragma warning disable CS8618
public class JwtOptions
{
    public string SigningKey { get; init; }
    public string Audience { get; init; }
    public string Issuer { get; init; }
    public TimeSpan TokenLifetime { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan RefreshTokenLifetime { get; init; } = TimeSpan.FromDays(7);
    public TimeSpan ClockSkew { get; init; } = TimeSpan.Zero;

    [UsedImplicitly]
    public class Validator : AbstractValidator<JwtOptions>
    {
        public Validator()
        {
            RuleFor(x => x.SigningKey).MinimumLength(32);
            RuleFor(x => x.Audience).NotEmpty();
            RuleFor(x => x.Issuer).NotEmpty();
            RuleFor(x => x.TokenLifetime).GreaterThan(TimeSpan.Zero);
            RuleFor(x => x.RefreshTokenLifetime).GreaterThan(TimeSpan.Zero);
            RuleFor(x => x.ClockSkew).GreaterThanOrEqualTo(TimeSpan.Zero);
        }
    }
}
