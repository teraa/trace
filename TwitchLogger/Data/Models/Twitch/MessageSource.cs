#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Twitch;

public class MessageSource
{
    public short Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Message> Messages { get; set; }
}
