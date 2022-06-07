// ReSharper disable UnusedAutoPropertyAccessor.Global

using JetBrains.Annotations;

#pragma warning disable CS8618
namespace TwitchLogger.Options;

public class PubSubOptions
{
    public string Token { get; init; }
}

public class ChatOptions
{
    public string MessageSourceName { get; init; }
}

[UsedImplicitly]
public class DbOptions
{
    public string ConnectionString { get; init; }
}
