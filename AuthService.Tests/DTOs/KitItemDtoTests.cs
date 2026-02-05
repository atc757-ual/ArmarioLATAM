using Xunit;
using AuthService.API.Dtos;

namespace AuthService.Tests.DTOs
{
    public class KitItemDtoTests
    {
        [Fact]
        public void KitItemDto_ShouldSetProperties()
        {
            var dto = new KitItemDto
            {
                GarmentId = 2,
                SizeId = 3,
                Quantity = 4
            };
            Assert.Equal(2, dto.GarmentId);
            Assert.Equal(3, dto.SizeId);
            Assert.Equal(4, dto.Quantity);
        }
    }
}
