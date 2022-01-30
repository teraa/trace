#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Twitch;

public class User
{
    public string Id { get; set; }
    public string Login { get; set; }
    public DateTimeOffset FirstSeenAt { get; set; }

    public ICollection<Message> AuthorMessages { get; set; }
    public ICollection<Message> ChannelMessages { get; set; }
    public ICollection<ModeratorAction> ChannelModeratorActions { get; set; }
    public ICollection<ModeratorAction> ModeratorModeratorActions { get; set; }
    public ICollection<ModeratorAction> TargetModeratorActions { get; set; }
    public ICollection<ChatLog> ChatLogs { get; set; }
    public ICollection<PubSubLog> PubSubLogs { get; set; }
}
