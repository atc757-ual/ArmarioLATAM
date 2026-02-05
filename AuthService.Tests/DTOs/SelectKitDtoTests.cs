using Xunit;
using AuthService.API.Dtos;

namespace AuthService.Tests.DTOs
{
    public class SelectKitDtoTests
    {
        [Fact]
        public void SelectKitDto_ShouldSetProperties()
        {
            var dto = new SelectKitDto
            {
                KitTypeId = 5
            };
            Assert.Equal(5, dto.KitTypeId);
        }
    }
}
