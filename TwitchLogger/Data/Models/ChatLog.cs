using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models;
using TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models
{
    [PublicAPI]
    public class ChatLog
    {
        public int Id { get; set; }
        public string ChannelId { get; set; }

        public User Channel { get; set; }
    }

    public class ChatLogConfiguration : IEntityTypeConfiguration<ChatLog>
    {
        public void Configure(EntityTypeBuilder<ChatLog> builder)
        {
            builder.HasIndex(x => x.ChannelId)
                .IsUnique();
        }
    }
}

namespace TwitchLogger.Data
{
    public partial class TwitchLoggerDbContext
    {
        public DbSet<ChatLog> ChatLogs { get; init; }
    }
}
