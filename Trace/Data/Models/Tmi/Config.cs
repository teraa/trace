using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Data.Models.Tmi;

#pragma warning disable CS8618
namespace Trace.Data.Models.Tmi
{
    [PublicAPI]
    public class Config
    {
        public int Id { get; set; }
        public string ChannelId { get; set; }
        public string ChannelLogin { get; set; }
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

namespace Trace.Data
{
    public partial class TraceDbContext
    {
        public DbSet<Config> TmiConfigs { get; init; }
    }
}
