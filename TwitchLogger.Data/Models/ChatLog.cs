using TwitchLogger.Data.Models.Twitch;

namespace TwitchLogger.Data.Models;

public class ChatLog
{
    public int Id { get; set; }
    public string ChannelId { get; set; } = null!;

    public User Channel { get; set; } = null!;
}
