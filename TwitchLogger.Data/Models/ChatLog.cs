using TwitchLogger.Data.Models.Twitch;

namespace TwitchLogger.Data.Models
{
    public class ChatLog : EntityBase<int>
    {
        public string ChannelId { get; set; } = null!;

        public virtual User Channel { get; set; } = null!;
    }
}
