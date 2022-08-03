using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Data.Models;

#pragma warning disable CS8618
namespace Trace.Data.Models
{
    [PublicAPI]
    public class ChannelPermission
    {
        public string UserId { get; set; }
        public string ChannelId { get; set; }
    }

    public class ChannelPermissionConfiguration : IEntityTypeConfiguration<ChannelPermission>
    {
        public void Configure(EntityTypeBuilder<ChannelPermission> builder)
        {
            builder.HasKey(x => new {UserTwitchId = x.UserId, ChannelTwitchId = x.ChannelId});
        }
    }
}

namespace Trace.Data
{
    public partial class TraceDbContext
    {
        public DbSet<ChannelPermission> ChannelPermissions { get; init; }
    }
}
