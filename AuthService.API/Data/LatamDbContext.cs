// Archivo: AuthService.API/Data/LatamDbContext.cs
using Microsoft.EntityFrameworkCore;
using AuthService.API.Models;

namespace AuthService.API.Data;

public class LatamDbContext : DbContext
{
    public LatamDbContext(DbContextOptions<LatamDbContext> options)
        : base(options) { }

    public DbSet<Garment> Garments => Set<Garment>();
    public DbSet<Size> Sizes => Set<Size>();
    public DbSet<KitType> KitTypes => Set<KitType>();
    public DbSet<Kit> Kits => Set<Kit>();
    public DbSet<KitItem> KitItems => Set<KitItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Garments
        modelBuilder.Entity<Garment>(entity =>
        {
            entity.HasKey(e => e.GarmentId);
            entity.ToTable("Garments");
        });

        // Sizes
        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId);
            entity.ToTable("Sizes");
        });

        // KitTypes
        modelBuilder.Entity<KitType>(entity =>
        {
            entity.HasKey(e => e.KitTypeId);
            entity.ToTable("KitTypes");
        });

        // Kits
        modelBuilder.Entity<Kit>(entity =>
        {
            entity.HasKey(e => e.KitId);
            entity.ToTable("Kits");

            entity.HasOne(k => k.KitType)
                .WithMany()
                .HasForeignKey(k => k.KitTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(k => k.Items)
                .WithOne(ki => ki.Kit)
                .HasForeignKey(ki => ki.KitId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // KitItems
        modelBuilder.Entity<KitItem>(entity =>
        {
            entity.HasKey(e => e.KitItemId);
            entity.ToTable("KitItems");

            entity.HasOne(ki => ki.Garment)
                .WithMany()
                .HasForeignKey(ki => ki.GarmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ki => ki.Size)
                .WithMany()
                .HasForeignKey(ki => ki.SizeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}