using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models.Tmi;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Tmi
{
    [PublicAPI]
    public class Source
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<Message> Messages { get; set; }
    }

    public class SourceConfiguration : IEntityTypeConfiguration<Source>
    {
        public void Configure(EntityTypeBuilder<Source> builder)
        {
            builder.Metadata.SetSchema("tmi");
            builder.Metadata.SetTableName("sources");

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}

namespace TwitchLogger.Data
{
    public partial class TwitchLoggerDbContext
    {
        public DbSet<Source> TmiSources { get; init; }
    }
}
