using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Trace.Api.Options;

namespace Trace.Api;

public class JwtSigningKeyProvider : IDisposable
{
    private readonly IDisposable _optionsChangeListener;

    public SymmetricSecurityKey Key { get; private set; }

    public JwtSigningKeyProvider(IOptionsMonitor<JwtOptions> options)
    {
        _optionsChangeListener = options.OnChange(UpdateKey);
        UpdateKey(options.CurrentValue, null);
    }

    [MemberNotNull(nameof(Key))]
    private void UpdateKey(JwtOptions options, string? name)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(options.SigningKey);
        Key = new SymmetricSecurityKey(bytes);
    }

    public void Dispose()
    {
        _optionsChangeListener.Dispose();
    }
}
