using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Data.Models.Tmi;

#pragma warning disable CS8618
namespace Trace.Data.Models.Tmi
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

namespace Trace.Data
{
    public partial class TraceDbContext
    {
        public DbSet<Source> TmiSources { get; init; }
    }
}
