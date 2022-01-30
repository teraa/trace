using TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models;

public class PubSubLog
{
    public int Id { get; set; }
    public string Topic { get; set; }
    public string ChannelId { get; set; }

    public User Channel { get; set; }
}
