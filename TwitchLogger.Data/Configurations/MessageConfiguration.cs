using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models.Twitch;

namespace TwitchLogger.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasIndex(x => x.ReceivedAt);

        builder.HasOne(x => x.Author)
            .WithMany(x => x.Messages)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Channel)
            .WithMany(x => x.ChannelMessages)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Source)
            .WithMany(x => x.Messages)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
