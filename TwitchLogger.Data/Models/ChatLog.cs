using TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models;

public class ChatLog
{
    public int Id { get; set; }
    public string ChannelId { get; set; }

    public User Channel { get; set; }
}
