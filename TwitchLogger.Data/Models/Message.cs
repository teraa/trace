using System;

namespace TwitchLogger.Data.Models
{
    public class Message : EntityBase<long>
    {
        public DateTimeOffset ReceivedAt { get; set; }
        public short SourceId { get; set; }
        public string ChannelId { get; set; } = null!;
        public string? AuthorId { get; set; } = null!;
        public string AuthorLogin { get; set; } = null!;
        public string Content { get; set; } = null!;

        public virtual User Author { get; set; } = null!;
        public virtual User Channel { get; set; } = null!;
        public virtual MessageSource Source { get; set; } = null!;
    }
}
