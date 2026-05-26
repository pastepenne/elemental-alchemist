using ElementalAlchemist.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ElementalAlchemist.Server.Database;

/// <summary>EF Core context backing the SQLite fusions store.</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Fusion> Fusions => Set<Fusion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Columns and table name use EF's PascalCase conventions; only the
        // order-independent uniqueness of a pair needs explicit declaration.
        modelBuilder.Entity<Fusion>()
            .HasIndex(x => new { x.ElementA, x.ElementB })
            .IsUnique();
    }
}
