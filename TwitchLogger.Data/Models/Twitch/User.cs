using System;
using System.Collections.Generic;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Twitch;

public class User
{
    public string Id { get; set; }
    public string Login { get; set; }
    public DateTimeOffset FirstSeenAt { get; set; }

    public ICollection<Message> Messages { get; set; }
    public ICollection<Message> ChannelMessages { get; set; }
    public ICollection<ModeratorAction> ChannelModeratorActions { get; set; }
    public ICollection<ModeratorAction> ModeratorActionsIssued { get; set; }
    public ICollection<ModeratorAction> ModeratorActionsReceived { get; set; }
    public ICollection<ChatLog> ChatLogs { get; set; }
    public ICollection<PubSubLog> PubSubLogs { get; set; }
}
