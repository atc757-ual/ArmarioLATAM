using Microsoft.EntityFrameworkCore;
using AuthService.API.Models;

namespace AuthService.API.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Configurar Id como auto-incremental
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(); // Para SQL Server
            
            // Email único
            entity.HasIndex(e => e.Email).IsUnique();
            
            // Configuraciones adicionales
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });
    }
}