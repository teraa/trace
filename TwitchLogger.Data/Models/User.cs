using System;
using System.Collections.Generic;

namespace TwitchLogger.Data.Models
{
    public class User : EntityBase<string>
    {
        public User()
        {
            Messages = new HashSet<Message>();
            ChannelMessages = new HashSet<Message>();
        }

        public string Login { get; set; } = null!;
        public DateTimeOffset FirstSeenAt { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = null!;
        public virtual ICollection<Message> ChannelMessages { get; set; } = null!;
    }
}
