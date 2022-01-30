using System.Collections.Generic;

namespace TwitchLogger.Data.Models.Twitch;

    public class MessageSource
    {
        public MessageSource()
        {
            Messages = new HashSet<Message>();
        }

        public short Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null!;

        public ICollection<Message> Messages { get; set; }
    }
