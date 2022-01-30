using System;

namespace TwitchLogger.Data.Models.Twitch;

    public class Message
    {
        public long Id { get; set; }
        public DateTimeOffset ReceivedAt { get; set; }
        public short SourceId { get; set; }
        public string ChannelId { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public string AuthorLogin { get; set; } = null!;
        public string Content { get; set; } = null!;

        public User Author { get; set; } = null!;
        public User Channel { get; set; } = null!;
        public MessageSource Source { get; set; } = null!;
    }
