using Xunit;
using AuthService.API.Dtos;
using System.Collections.Generic;

namespace AuthService.Tests.DTOs
{
    public class CreateKitDtoTests
    {
        [Fact]
        public void CreateKitDto_ShouldInitializeList()
        {
            var dto = new CreateKitDto();
            Assert.NotNull(dto.Items);
            Assert.Empty(dto.Items);
        }
    }
}
