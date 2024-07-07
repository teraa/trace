namespace Trace.Api;

public static class AppClaimTypes
{
    /// <summary>
    /// Authenticated user's Twitch account ID.
    /// </summary>
    public const string TwitchId = "twitch.id";
    /// <summary>
    /// Authenticated user's Twitch account login.
    /// </summary>
    public const string TwitchLogin = "twitch.login";
    /// <summary>
    /// Permission to read channel's data.
    /// </summary>
    public const string ChannelRead = "channel.read";
}
