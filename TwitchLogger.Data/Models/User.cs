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
            ChannelModeratorActions = new HashSet<ModeratorAction>();
            ModeratorActionsIssued = new HashSet<ModeratorAction>();
            ModeratorActionsReceived = new HashSet<ModeratorAction>();
        }

        public string Login { get; set; } = null!;
        public DateTimeOffset FirstSeenAt { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Message> ChannelMessages { get; set; }
        public virtual ICollection<ModeratorAction> ChannelModeratorActions { get; set; }
        public virtual ICollection<ModeratorAction> ModeratorActionsIssued { get; set; }
        public virtual ICollection<ModeratorAction> ModeratorActionsReceived { get; set; }
    }
}
