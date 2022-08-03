using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Trace.Api.Options;

namespace Trace.Api.Features.Auth;

public class TokenService
{
    private readonly IOptionsMonitor<JwtOptions> _options;
    private readonly JwtSigningKeyProvider _signingKeyProvider;

    public TokenService(IOptionsMonitor<JwtOptions> options, JwtSigningKeyProvider signingKeyProvider)
    {
        _options = options;
        _signingKeyProvider = signingKeyProvider;
    }

    public TokenData CreateToken(DateTimeOffset issuedAt, Guid userId)
    {
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                // new(JwtRegisteredClaimNames.Name, username),
            }),
            Expires = issuedAt.UtcDateTime + _options.CurrentValue.TokenLifetime,
            SigningCredentials = new SigningCredentials(
                key: _signingKeyProvider.Key,
                algorithm: SecurityAlgorithms.HmacSha256Signature),
            Audience = _options.CurrentValue.Audience,
            Issuer = _options.CurrentValue.Issuer,
            IssuedAt = issuedAt.UtcDateTime
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string tokenValue = tokenHandler.WriteToken(token);

        return new TokenData(
            (tokenValue, _options.CurrentValue.TokenLifetime),
            (Guid.NewGuid(), _options.CurrentValue.RefreshTokenLifetime));
    }

    public bool IsValid(DateTimeOffset checkAt, DateTimeOffset expiresAt)
        => checkAt + _options.CurrentValue.ClockSkew < expiresAt;

    public record TokenData(
        (string Value, TimeSpan ExpiresIn) Token,
        (Guid Value, TimeSpan ExpiresIn) RefreshToken);
}
