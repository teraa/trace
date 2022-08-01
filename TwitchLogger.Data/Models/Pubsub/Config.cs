using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Data.Models.Pubsub;

#pragma warning disable CS8618
namespace Trace.Data.Models.Pubsub
{
    [PublicAPI]
    public class Config
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public string ChannelId { get; set; }
    }

    public class ConfigConfiguration : IEntityTypeConfiguration<Config>
    {
        public void Configure(EntityTypeBuilder<Config> builder)
        {
            builder.Metadata.SetSchema("pubsub");
            builder.Metadata.SetTableName("configs");

            builder.HasIndex(x => x.Topic)
                .IsUnique();
        }
    }
}

namespace Trace.Data
{
    public partial class TraceDbContext
    {
        public DbSet<Config> PubsubConfigs { get; init; }
    }
}
