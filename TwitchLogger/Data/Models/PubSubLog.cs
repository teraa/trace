using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models;
using TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models
{
    [PublicAPI]
    public class PubSubLog
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public string ChannelId { get; set; }

        public User Channel { get; set; }
    }

    public class PubSubLogConfiguration : IEntityTypeConfiguration<PubSubLog>
    {
        public void Configure(EntityTypeBuilder<PubSubLog> builder)
        {
            builder.HasIndex(x => x.Topic)
                .IsUnique();
        }
    }
}

namespace TwitchLogger.Data
{
    public partial class TwitchLoggerDbContext
    {
        public DbSet<PubSubLog> PubSubLogs { get; init; }
    }
}
