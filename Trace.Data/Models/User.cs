using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Data.Models;

#pragma warning disable CS8618
namespace Trace.Data.Models
{
    [PublicAPI]
    public class User
    {
        public Guid Id { get; set; }
        public string TwitchId { get; set; }
        public string TwitchLogin { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(x => x.TwitchId)
                .IsUnique();
        }
    }
}

namespace Trace.Data
{
    public partial class TraceDbContext
    {
        public DbSet<User> Users { get; init; }
    }
}
