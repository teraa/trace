// ReSharper disable UnusedAutoPropertyAccessor.Global

using JetBrains.Annotations;

#pragma warning disable CS8618
namespace Trace;

public class PubSubOptions
{
    public string Token { get; init; }
}

public class TmiOptions
{
    public string MessageSourceName { get; init; }
    public string Login { get; init; }
    public string Token { get; init; }
}

[UsedImplicitly]
public class DbOptions
{
    public string ConnectionString { get; init; }
}
