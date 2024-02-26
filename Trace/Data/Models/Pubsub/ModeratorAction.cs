using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Data.Models.Pubsub;

#pragma warning disable CS8618
namespace Trace.Data.Models.Pubsub
{
    [PublicAPI]
    public sealed class ModeratorAction
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ChannelId { get; set; }
        public string Action { get; set; }
        public string InitiatorId { get; set; }
        public string InitiatorName { get; set; }

        // Term
        public string? TermId { get; set; }
        public string? TermText { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        // Unban request
        public string? ModeratorMessage { get; set; }

        // Timeout, Followers, Slow
        public TimeSpan? Duration { get; set; }

        // Timeout, Ban
        public string? Reason { get; set; }

        // Targeted
        public string? TargetId { get; set; }

        // Targeted, Raid
        public string? TargetName { get; set; }

        // Delete
        public string? MessageId { get; set; }
        public string? Message { get; set; }

        // LowUserTrust
        public string? LowUserTrustTreatment { get; set; }
        public List<string>? LowUserTrustTypes { get; set; }
        public string? LowUserTrustBanEvasionEvaluation { get; set; }
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
            builder.HasIndex(x => x.TargetId);
        }
    }
}

namespace Trace.Data
{
    public partial class AppDbContext
    {
        public DbSet<ModeratorAction> ModeratorActions { get; init; }
    }
}
