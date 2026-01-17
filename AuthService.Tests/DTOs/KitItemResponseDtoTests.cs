using Xunit;
using AuthService.API.Dtos;

namespace AuthService.Tests.DTOs
{
    public class KitItemResponseDtoTests
    {
        [Fact]
        public void KitItemResponseDto_ShouldSetProperties()
        {
            var dto = new KitItemResponseDto
            {
                GarmentName = "Playera",
                SizeCode = "M",
                Quantity = 2
            };
            Assert.Equal("Playera", dto.GarmentName);
            Assert.Equal("M", dto.SizeCode);
            Assert.Equal(2, dto.Quantity);
        }
    }
}
