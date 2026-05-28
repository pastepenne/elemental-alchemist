using Microsoft.EntityFrameworkCore;

namespace ElementalAlchemist.Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Element> Elements => Set<Element>();
    public DbSet<Fusion> Fusions => Set<Fusion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Element>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(64);
            e.Property(x => x.DisplayName).HasMaxLength(128);
            e.Property(x => x.Description).HasMaxLength(1024);
            e.Property(x => x.Tier).HasConversion<string>().HasMaxLength(32);
        });
        
        modelBuilder.Entity<Fusion>(f =>
        {
            f.HasKey(x => x.Id);
            f.Property(x => x.ElementA).HasMaxLength(64);
            f.Property(x => x.ElementB).HasMaxLength(64);
            f.Property(x => x.ResultId).HasMaxLength(64);
            f.HasIndex(x => new { x.ElementA, x.ElementB }).IsUnique();

            f.HasOne(x => x.Result)
                .WithMany()
                .HasForeignKey(x => x.ResultId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
