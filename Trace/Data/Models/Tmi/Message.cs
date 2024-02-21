using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Data.Models.Tmi;

#pragma warning disable CS8618
namespace Trace.Data.Models.Tmi
{
    [PublicAPI]
    public class Message
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public short SourceId { get; set; }
        public string ChannelId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorLogin { get; set; }
        public string Content { get; set; }

        public Source Source { get; set; }
    }

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.Metadata.SetSchema("tmi");
            builder.Metadata.SetTableName("messages");

            builder.HasIndex(x => x.Timestamp);
            builder.HasIndex(x => new {x.ChannelId, x.Timestamp});
            builder.HasIndex(x => new {x.ChannelId, x.AuthorId, x.Timestamp});

            builder.HasOne(x => x.Source)
                .WithMany(x => x.Messages)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

namespace Trace.Data
{
    public partial class AppDbContext
    {
        public DbSet<Message> TmiMessages { get; init; }
    }
}
