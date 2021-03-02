using System;
using System.Collections.Generic;

namespace TwitchLogger.Data.Models.Twitch
{
    public class User
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

        public string Id { get; set; } = null!;
        public string Login { get; set; } = null!;
        public DateTimeOffset FirstSeenAt { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<Message> ChannelMessages { get; set; }
        public ICollection<ModeratorAction> ChannelModeratorActions { get; set; }
        public ICollection<ModeratorAction> ModeratorActionsIssued { get; set; }
        public ICollection<ModeratorAction> ModeratorActionsReceived { get; set; }

        public ICollection<ChatLog> ChatLogs { get; set; }
        public ICollection<PubSubLog> PubSubLogs { get; set; }
    }
}
