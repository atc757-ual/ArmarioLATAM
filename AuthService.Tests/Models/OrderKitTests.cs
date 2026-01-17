using AuthService.API.Models;
using Xunit;

namespace AuthService.Tests.Models
{
    public class OrderKitTests
    {
        [Fact]
        public void OrderKit_Should_Set_And_Get_Properties()
        {
            var orderKit = new OrderKit
            {
                OrderKitId = 1,
                OrderId = 1,
                Order = new Order { OrderId = 1 },
                GarmentId = 1,
                Garment = new Garment { GarmentId = 1, Name = "Shirt" },
                Size = "M",
                Quantity = 2,
                Price = 25.5m,
                Languages = "EN"
            };

            Assert.Equal(1, orderKit.OrderKitId);
            Assert.Equal(1, orderKit.OrderId);
            Assert.Equal(1, orderKit.GarmentId);
            Assert.Equal("Shirt", orderKit.Garment.Name);
            Assert.Equal("M", orderKit.Size);
            Assert.Equal(2, orderKit.Quantity);
            Assert.Equal(25.5m, orderKit.Price);
            Assert.Equal("EN", orderKit.Languages);
        }
    }
}
