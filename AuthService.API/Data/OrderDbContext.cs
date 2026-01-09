using Microsoft.EntityFrameworkCore;
using AuthService.API.Models;

namespace AuthService.API.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
}