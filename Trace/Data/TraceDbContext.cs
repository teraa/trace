using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Teraa.Extensions.Identity;

#pragma warning disable CS8618
namespace Trace.Data;

public partial class TraceDbContext : IdentityDbContext<AppUser>
{
    public TraceDbContext(DbContextOptions<TraceDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TraceDbContext).Assembly);
        modelBuilder.UseSnakeCaseIdentityNames<AppUser>();
    }
}

public sealed class AppUser : IdentityUser;
