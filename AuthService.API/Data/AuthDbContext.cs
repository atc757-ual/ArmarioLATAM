// AuthService.API.Data/AuthDbContext.cs
using Microsoft.EntityFrameworkCore;
using AuthService.API.Models;

namespace AuthService.API.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    public DbSet<ResetPassword> ResetPassword { get; set; } = null!;
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .ValueGeneratedNever();

            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Email)
                  .IsRequired()
                  .HasMaxLength(256);

            entity.Property(e => e.Name)          // ✅ NUEVO
                  .IsRequired()
                  .HasMaxLength(250);

            entity.Property(e => e.BP)      // ✅ NUEVO
                  .IsRequired()
                  .HasMaxLength(10);

            entity.Property(e => e.IsActive)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .IsRequired();
        });
    }
}