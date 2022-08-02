using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Trace.Api.Options;

namespace Trace.Api.Features.Auth;

public class TokenService : IDisposable
{
    private readonly IOptionsMonitor<JwtOptions> _options;
    private readonly IDisposable _optionsChangeListener;
    private SymmetricSecurityKey _signingKey;

    public TokenService(IOptionsMonitor<JwtOptions> options)
    {
        _options = options;
        _optionsChangeListener = options.OnChange(UpdateKey);
        UpdateKey(options.CurrentValue, null);
    }

    [MemberNotNull(nameof(_signingKey))]
    private void UpdateKey(JwtOptions options, string? name)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(options.SigningKey);
        _signingKey = new SymmetricSecurityKey(bytes);
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
                key: _signingKey,
                algorithm: SecurityAlgorithms.HmacSha256Signature),
            Audience = _options.CurrentValue.Audience,
            Issuer = _options.CurrentValue.Issuer,
            IssuedAt = issuedAt.UtcDateTime
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string tokenValue = tokenHandler.WriteToken(token);

        return new TokenData(tokenValue, _options.CurrentValue.TokenLifetime);
    }

    public RefreshTokenData CreateRefreshToken()
    {
        return new RefreshTokenData(Guid.NewGuid(), _options.CurrentValue.RefreshTokenLifetime);
    }

    public void Dispose()
    {
        _optionsChangeListener.Dispose();
    }

    public record TokenData(string Value, TimeSpan ExpiresIn);

    public record RefreshTokenData(Guid Value, TimeSpan ExpiresIn);
}
