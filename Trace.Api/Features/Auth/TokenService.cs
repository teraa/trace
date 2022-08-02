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

    public string CreateToken(Guid userId)
    {
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                // new(JwtRegisteredClaimNames.Name, username),
            }),
            Expires = DateTime.UtcNow + _options.CurrentValue.TokenLifetime,
            SigningCredentials = new SigningCredentials(
                key: _signingKey,
                algorithm: SecurityAlgorithms.HmacSha256Signature),
            Audience = _options.CurrentValue.Audience,
            Issuer = _options.CurrentValue.Issuer,
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public void Dispose()
    {
        _optionsChangeListener.Dispose();
    }
}
