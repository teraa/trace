using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Trace.Options;

namespace Trace.Api;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IOptionsMonitor<JwtOptions> _options;
    private readonly JwtSigningKeyProvider _signingKeyProvider;

    public ConfigureJwtBearerOptions(IOptionsMonitor<JwtOptions> options, JwtSigningKeyProvider signingKeyProvider)
    {
        _options = options;
        _signingKeyProvider = signingKeyProvider;
    }

    public void Configure(JwtBearerOptions options)
    {
        var jwtOptions = _options.CurrentValue;

        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = _signingKeyProvider.Key,
            ValidAudience = jwtOptions.Audience,
            ValidIssuer = jwtOptions.Issuer,
            ClockSkew = jwtOptions.ClockSkew,
        };
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);

        if (name == AppAuthScheme.ExpiredBearer)
        {
            options.TokenValidationParameters.ValidateLifetime = false;
        }
    }
}
