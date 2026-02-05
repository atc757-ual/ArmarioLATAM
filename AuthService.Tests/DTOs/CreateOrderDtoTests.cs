using Xunit;
using AuthService.API.Dtos;
using System.Collections.Generic;

namespace AuthService.Tests.DTOs
{
    public class CreateOrderDtoTests
    {
        [Fact]
        public void CreateOrderDto_ShouldInitializeList()
        {
            var dto = new CreateOrderDto();
            Assert.NotNull(dto.Items);
            Assert.Empty(dto.Items);
        }
    }
}
