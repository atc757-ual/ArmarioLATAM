using AuthService.API.Models;
using Xunit;

namespace AuthService.Tests.Models
{
    public class KitItemTests
    {
        [Fact]
        public void KitItem_Should_Set_And_Get_Properties_Correctly()
        {
            // Arrange
            var garment = new Garment
            {
                Name = "T-Shirt",
                Description = "Cotton T-Shirt",
                Price = 19.99m
            };

            var size = new Size
            {
                Code = "M",
                Description = "Medium"
            };

            var kitItem = new KitItem
            {
                KitItemId = 1,
                KitId = 100,
                GarmentId = 200,
                SizeId = 300,
                Quantity = 5,
                Garment = garment,
                Size = size
            };

            // Act & Assert
            Assert.Equal(1, kitItem.KitItemId);
            Assert.Equal(100, kitItem.KitId);
            Assert.Equal(200, kitItem.GarmentId);
            Assert.Equal(300, kitItem.SizeId);
            Assert.Equal(5, kitItem.Quantity);

            // Propiedades del Garment
            Assert.Equal("T-Shirt", kitItem.Garment?.Name);
            Assert.Equal("Cotton T-Shirt", kitItem.Garment?.Description);
            Assert.Equal(19.99m, kitItem.Garment?.Price);

            // Propiedades del Size
            Assert.Equal("M", kitItem.Size?.Code);
            Assert.Equal("Medium", kitItem.Size?.Description);
        }
    }
}
