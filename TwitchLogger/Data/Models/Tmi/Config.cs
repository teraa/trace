using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models.Tmi;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Tmi
{
    [PublicAPI]
    public class Config
    {
        public int Id { get; set; }
        public string ChannelId { get; set; }

        public User Channel { get; set; }
    }

    public class ConfigConfiguration : IEntityTypeConfiguration<Config>
    {
        public void Configure(EntityTypeBuilder<Config> builder)
        {
            builder.Metadata.SetSchema("tmi");
            builder.Metadata.SetTableName("configs");

            builder.HasIndex(x => x.ChannelId)
                .IsUnique();
        }
    }
}

namespace TwitchLogger.Data
{
    public partial class TwitchLoggerDbContext
    {
        public DbSet<Config> TmiConfigs { get; init; }
    }
}
