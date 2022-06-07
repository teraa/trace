using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models.Twitch;

namespace TwitchLogger.Data.Configurations;

public class ModeratorActionConfiguration : IEntityTypeConfiguration<ModeratorAction>
{
    public void Configure(EntityTypeBuilder<ModeratorAction> builder)
    {
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
