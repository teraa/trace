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
