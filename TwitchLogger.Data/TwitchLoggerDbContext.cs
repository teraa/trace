using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618
namespace Trace.Data;

[PublicAPI]
public partial class TraceDbContext : DbContext
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
    }
}
