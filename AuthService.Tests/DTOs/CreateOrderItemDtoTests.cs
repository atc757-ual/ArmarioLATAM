using Xunit;
using AuthService.API.Dtos;

namespace AuthService.Tests.DTOs
{
    public class CreateOrderItemDtoTests
    {
        [Fact]
        public void CreateOrderItemDto_ShouldSetProperties()
        {
            var dto = new CreateOrderItemDto
            {
                GarmentId = 1,
                Size = "M",
                Quantity = 5
            };
            Assert.Equal(1, dto.GarmentId);
            Assert.Equal("M", dto.Size);
            Assert.Equal(5, dto.Quantity);
        }
    }
}
