using System;
using System.Collections.Generic;

namespace TwitchLogger.Data.Models.Twitch
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
            ChatLogs = new HashSet<ChatLog>();
            PubSubLogs = new HashSet<PubSubLog>();
        }

        public string Login { get; set; } = null!;
        public DateTimeOffset FirstSeenAt { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Message> ChannelMessages { get; set; }
        public virtual ICollection<ModeratorAction> ChannelModeratorActions { get; set; }
        public virtual ICollection<ModeratorAction> ModeratorActionsIssued { get; set; }
        public virtual ICollection<ModeratorAction> ModeratorActionsReceived { get; set; }

        public virtual ICollection<ChatLog> ChatLogs { get; set; }
        public virtual ICollection<PubSubLog> PubSubLogs { get; set; }
    }
}
