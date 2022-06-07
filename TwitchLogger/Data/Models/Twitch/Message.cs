#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Twitch;

public class Message
{
    public long Id { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public short SourceId { get; set; }
    public string ChannelId { get; set; }
    public string AuthorId { get; set; }
    public string AuthorLogin { get; set; }
    public string Content { get; set; }

    public User Author { get; set; }
    public User Channel { get; set; }
    public MessageSource Source { get; set; }
}
