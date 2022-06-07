using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models;
using TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models
{
    [PublicAPI]
    public class TmiConfig
    {
        public int Id { get; set; }
        public string ChannelId { get; set; }

        public User Channel { get; set; }
    }

    public class TmiConfigConfiguration : IEntityTypeConfiguration<TmiConfig>
    {
        public void Configure(EntityTypeBuilder<TmiConfig> builder)
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
        public DbSet<TmiConfig> TmiConfigs { get; init; }
    }
}
