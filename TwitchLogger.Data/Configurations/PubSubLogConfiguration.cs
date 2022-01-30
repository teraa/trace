using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models;

namespace TwitchLogger.Data.Configurations;

public class PubSubLogConfiguration : IEntityTypeConfiguration<PubSubLog>
{
    public void Configure(EntityTypeBuilder<PubSubLog> builder)
    {
        builder.HasIndex(x => x.Topic)
            .IsUnique();
    }
}
