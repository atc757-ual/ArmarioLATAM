using AuthService.API.Models;
using Xunit;

namespace AuthService.Tests.Models
{
    public class OrderTests
    {
        [Fact]
        public void Order_Should_Set_And_Get_Properties_Correctly()
        {
            var order = new Order
            {
                OrderId = 1,
                UserId = Guid.NewGuid(),
                DateOrder = DateTime.Now,
                TotalPrice = 100.50m,
                Status = "Pending",
                KitTypeId = 1,
                KitType = new KitType { KitTypeId = 1, Name = "Basic" }
            };

            Assert.Equal(1, order.OrderId);
            Assert.Equal(1, order.KitTypeId);
            Assert.Equal("Basic", order.KitType.Name);
            Assert.Equal("Pending", order.Status);
        }
    }
}
