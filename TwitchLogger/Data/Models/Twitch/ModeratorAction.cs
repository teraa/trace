using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Twitch
{
    [PublicAPI]
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

    public class ModeratorActionConfiguration : IEntityTypeConfiguration<ModeratorAction>
    {
        public void Configure(EntityTypeBuilder<ModeratorAction> builder)
        {
            builder.Metadata.SetSchema("twitch");

            builder.HasIndex(x => x.CreatedAt);

            builder.HasOne(x => x.Channel)
                .WithMany(x => x.ChannelModeratorActions)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Moderator)
                .WithMany(x => x.ModeratorModeratorActions)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Target)
                .WithMany(x => x.TargetModeratorActions)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

namespace TwitchLogger.Data
{
    public partial class TwitchLoggerDbContext
    {
        public DbSet<ModeratorAction> ModeratorActions { get; init; }
    }
}
