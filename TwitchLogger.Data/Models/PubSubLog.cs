using TwitchLogger.Data.Models.Twitch;

namespace TwitchLogger.Data.Models
{
    public class PubSubLog : EntityBase<int>
    {
        public string Topic { get; set; } = null!;
        public string ChannelId { get; set; } = null!;

        public User Channel { get; set; } = null!;
    }
}
