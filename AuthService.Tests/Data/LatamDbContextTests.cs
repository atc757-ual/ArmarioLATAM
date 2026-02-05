// AuthService.Tests/Data/LatamDbContextTests.cs
using Xunit;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;
using AuthService.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Tests.Data
{
    public class LatamDbContextTests
    {
        private LatamDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<LatamDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LatamDbContext(options);
        }

        [Fact]
        public async Task CanAddAndRetrieveKitTypes()
        {
            using var context = GetInMemoryDb();

            var kitType1 = new KitType { KitTypeId = 1, Name = "Kit A", IsActive = true };
            var kitType2 = new KitType { KitTypeId = 2, Name = "Kit B", IsActive = true };

            context.KitTypes.AddRange(kitType1, kitType2);
            await context.SaveChangesAsync();

            var kitTypes = context.KitTypes.ToList();
            Assert.Equal(2, kitTypes.Count);
            Assert.Contains(kitTypes, k => k.Name == "Kit A");
            Assert.Contains(kitTypes, k => k.Name == "Kit B");
        }

        [Fact]
        public async Task CanAddAndRetrieveGarments()
        {
            using var context = GetInMemoryDb();

            var garment1 = new Garment { GarmentId = 1, Name = "Playera", IsActive = true, Price = 10m };
            var garment2 = new Garment { GarmentId = 2, Name = "Pantalón", IsActive = true, Price = 20m };

            context.Garments.AddRange(garment1, garment2);
            await context.SaveChangesAsync();

            var garments = context.Garments.ToList();
            Assert.Equal(2, garments.Count);
            Assert.Contains(garments, g => g.Name == "Playera");
            Assert.Contains(garments, g => g.Name == "Pantalón");
        }

        [Fact]
        public async Task CanAddAndRetrieveOrderWithOrderKits()
        {
            using var context = GetInMemoryDb();

            // Crear KitType
            var kit = new KitType { KitTypeId = 1, Name = "Kit A", IsActive = true };
            context.KitTypes.Add(kit);

            // Crear Garment
            var garment = new Garment { GarmentId = 1, Name = "Playera", IsActive = true, Price = 15m };
            context.Garments.Add(garment);

            await context.SaveChangesAsync();

            // Crear Order
            var order = new Order
            {
                OrderId = 1,
                UserId = Guid.NewGuid(),
                DateOrder = DateTime.UtcNow,
                TotalPrice = 30m,
                Status = "Pending",
                KitTypeId = kit.KitTypeId,
                KitType = kit
            };

            // Crear OrderKit
            var orderKit = new OrderKit
            {
                OrderKitId = 1,
                Order = order,
                OrderId = order.OrderId,
                Garment = garment,
                GarmentId = garment.GarmentId,
                Quantity = 2,
                Price = 15m,
                Size = "M"
            };

            order.OrderKits.Add(orderKit);
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var retrievedOrder = context.Orders
                                        .Include(o => o.OrderKits)
                                        .ThenInclude(ok => ok.Garment)
                                        .FirstOrDefault(o => o.OrderId == 1);

            Assert.NotNull(retrievedOrder);
            Assert.Equal(30m, retrievedOrder.TotalPrice);
            Assert.Single(retrievedOrder.OrderKits);
            Assert.Equal("Playera", retrievedOrder.OrderKits.First().Garment.Name);
        }
    }
}
