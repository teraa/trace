using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models.Pubsub;
using Timeout = TwitchLogger.Data.Models.Pubsub.Timeout;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Pubsub
{
    [PublicAPI]
    public class ModeratorAction
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ChannelId { get; set; }
        public string Action { get; set; }
        public string InitiatorId { get; set; }
        public string InitiatorName { get; set; }
    }

    public class ModeratorActionConfiguration : IEntityTypeConfiguration<ModeratorAction>
    {
        public void Configure(EntityTypeBuilder<ModeratorAction> builder)
        {
            builder.Metadata.SetSchema("pubsub");

            builder.HasIndex(x => x.Timestamp);
            builder.HasIndex(x => x.Action);
            builder.HasIndex(x => x.ChannelId);
            builder.HasIndex(x => x.InitiatorId);
        }
    }

    [PublicAPI]
    public class TargetedModeratorAction : ModeratorAction
    {
        public string TargetId { get; set; }
        public string TargetName { get; set; }
    }

    public class TargetedModeratorActionConfiguration : IEntityTypeConfiguration<TargetedModeratorAction>
    {
        public void Configure(EntityTypeBuilder<TargetedModeratorAction> builder)
        {
            builder.HasIndex(x => x.TargetId);
        }
    }

    [PublicAPI]
    public class Ban : TargetedModeratorAction
    {
        public string Reason { get; set; }
    }

    [PublicAPI]
    public class Followers : ModeratorAction
    {
        public TimeSpan Duration { get; set; }
    }

    [PublicAPI]
    public class Raid : ModeratorAction
    {
        public string TargetName { get; set; }
    }

    [PublicAPI]
    public class Slow : ModeratorAction
    {
        public TimeSpan Duration { get; set; }
    }

    [PublicAPI]
    public class Timeout : TargetedModeratorAction
    {
        public TimeSpan Duration { get; set; }
        public string Reason { get; set; }
    }

    [PublicAPI]
    public class UnbanRequestAction : TargetedModeratorAction
    {
        public string ModeratorMessage { get; set; }
    }

    [PublicAPI]
    public class TermAction : ModeratorAction
    {
        public string TermId { get; set; }
        public string Text { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}

namespace TwitchLogger.Data
{
    public partial class TwitchLoggerDbContext
    {
        public DbSet<ModeratorAction> ModeratorActions { get; init; }

        public DbSet<Ban> Bans { get; init; }
        public DbSet<Followers> Followers { get; init; }
        public DbSet<Raid> Raids { get; init; }
        public DbSet<Slow> Slows { get; init; }
        public DbSet<Timeout> Timeouts { get; init; }
        public DbSet<UnbanRequestAction> UnbanRequestActions { get; init; }
        public DbSet<TermAction> TermActions { get; init; }
    }
}
