using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Trace.Data.Models;

#pragma warning disable CS8618
namespace Trace.Data.Models
{
    [PublicAPI]
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset IssuedAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }

        public User User { get; set; }
    }
}

namespace Trace.Data
{
    public partial class TraceDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; init; }
    }
}
