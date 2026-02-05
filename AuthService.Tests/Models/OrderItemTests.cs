using AuthService.API.Models;
using Xunit;

namespace AuthService.Tests.Models
{
    public class OrderItemTests
    {
        [Fact]
        public void OrderItem_Should_Set_And_Get_Properties()
        {
            var item = new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                ProductName = "Sample Product",
                Quantity = 5,
                Price = 10.0m
            };

            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.Equal("Sample Product", item.ProductName);
            Assert.Equal(5, item.Quantity);
            Assert.Equal(10.0m, item.Price);
        }
    }
}
