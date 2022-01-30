using System;
using System.Collections.Generic;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Twitch;

public class ModeratorAction
{
    public long Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string ChannelId { get; set; }
    public string Action { get; set; }
    public List<string>? Args { get; set; }
    public string? MessageId { get; set; }
    public string? ModeratorId { get; set; }
    public string? TargetId { get; set; }
    public string? TargetLogin { get; set; }
    public string? ModeratorMessage { get; set; }

    public User Channel { get; set; }
    public User? Moderator { get; set; }
    public User? Target { get; set; }
}
