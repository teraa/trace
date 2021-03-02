using System;
using System.Collections.Generic;

namespace TwitchLogger.Data.Models.Twitch
{
    public class ModeratorAction : EntityBase<long>
    {
        public DateTimeOffset CreatedAt { get; set; }
        public string ChannelId { get; set; } = null!;
        public string Action { get; set; } = null!;
        public List<string>? Args { get; set; } = null!;
        public string? MessageId { get; set; } = null!;
        public string? ModeratorId { get; set; }
        public string? TargetId { get; set; }
        public string? TargetLogin { get; set; }
        public string? ModeratorMessage { get; set; }

        public User Channel { get; set; } = null!;
        public User? Moderator { get; set; }
        public User? Target { get; set; }
    }
}
