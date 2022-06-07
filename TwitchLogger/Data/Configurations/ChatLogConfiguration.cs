using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models;

namespace TwitchLogger.Data.Configurations;

public class ChatLogConfiguration : IEntityTypeConfiguration<ChatLog>
{
    public void Configure(EntityTypeBuilder<ChatLog> builder)
    {
        builder.HasIndex(x => x.ChannelId)
            .IsUnique();
    }
}
