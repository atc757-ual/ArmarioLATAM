using Xunit;
using AuthService.API.Dtos;

namespace AuthService.Tests.DTOs
{
    public class OrderItemDtoTests
    {
        [Fact]
        public void OrderItemDto_ShouldSetProperties()
        {
            var dto = new OrderItemDto
            {
                ProductId = 10,
                Quantity = 3
            };
            Assert.Equal(10, dto.ProductId);
            Assert.Equal(3, dto.Quantity);
        }
    }
}
