using Xunit;
using AuthService.API.Dtos;
using System;

namespace AuthService.Tests.DTOs
{
    public class KitResponseDtoTests
    {
        [Fact]
        public void KitResponseDto_ShouldInitializeListAndSetProperties()
        {
            var dto = new KitResponseDto
            {
                KitId = 1,
                OrderNumber = "ORD123",
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                KitTypeName = "Kit A"
            };

            Assert.NotNull(dto.Items);
            Assert.Empty(dto.Items);
            Assert.Equal(1, dto.KitId);
            Assert.Equal("ORD123", dto.OrderNumber);
            Assert.Equal("Pending", dto.Status);
            Assert.Equal("Kit A", dto.KitTypeName);
        }
    }
}
