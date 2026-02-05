using AuthService.API.Dtos;
using AuthService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API.Data;

public class LatamDbContext : DbContext
{
    public LatamDbContext(DbContextOptions<LatamDbContext> options)
        : base(options) { }

    public DbSet<Garment> Garments => Set<Garment>();
    public DbSet<KitType> KitTypes => Set<KitType>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderKit> OrderKits => Set<OrderKit>();
    public DbSet<AddInfoOrder> AddInfoOrders => Set<AddInfoOrder>();
    public DbSet<Tracking> Tracking => Set<Tracking>();
    public DbSet<TrackingStatus> TrackingStatus => Set<TrackingStatus>();
    public DbSet<User> Users => Set<User>();  // o ApplicationUser si es AspNetUsers

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // aquí mapeas Users a la tabla correcta:
        modelBuilder.Entity<User>().ToTable("Users"); // o "AspNetUsers"

        // ========================
        // TABLAS
        // ========================
        modelBuilder.Entity<Garment>().ToTable("Garments");
        modelBuilder.Entity<KitType>().ToTable("KitTypes");
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<OrderKit>().ToTable("OrderKits");
        modelBuilder.Entity<AddInfoOrder>().ToTable("AddInfoOrder");
        modelBuilder.Entity<Tracking>().ToTable("Tracking");
        modelBuilder.Entity<TrackingStatus>().ToTable("TrackingStatus");

        modelBuilder.Entity<Garment>()
            .Property(g => g.Price)
            .HasPrecision(18, 2);          // decimal(18,2)

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderKit>()
            .Property(ok => ok.Price)
            .HasPrecision(18, 2);

        // ========================
        // ORDER -> ORDERKITS (1:N)
        // ========================
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderKits)
            .WithOne(ok => ok.Order)
            .HasForeignKey(ok => ok.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========================
        // ORDERKIT -> GARMENT (N:1)
        // ========================
        modelBuilder.Entity<OrderKit>()
            .HasOne(ok => ok.Garment)
            .WithMany()
            .HasForeignKey(ok => ok.GarmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
.HasOne(o => o.AddInfoOrder)
.WithOne(a => a.Order)
.HasForeignKey<AddInfoOrder>(a => a.OrderId);
        // Order borra → AddInfoOrder.OrderId=NULL

        // ========================
        // ORDER -> TRACKING (1:N) ✅ CORREGIDO
        // ========================
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Tracking)           // Order.Tracking (List<Tracking>)
            .WithOne(t => t.Order)              // Tracking.Order (Order property) ✅ NO t.OrderId
            .HasForeignKey(t => t.OrderId)      // FK campo
            .OnDelete(DeleteBehavior.Cascade);


    }
}
