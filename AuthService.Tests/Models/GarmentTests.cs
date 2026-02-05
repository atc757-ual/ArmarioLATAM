using AuthService.API.Models;
using Xunit;

namespace AuthService.Tests.Models
{
    public class GarmentTests
    {
        [Fact]
        public void Garment_Should_Set_And_Get_Properties_Correctly()
        {
            var garment = new Garment
            {
                GarmentId = 1,
                Name = "T-Shirt",
                Description = "Cotton T-Shirt",
                IsActive = true,
                ImageURL = "https://example.com/image.png",
                QuantityAuth = 50,
                Sizes = "S,M,L,XL",
                Languages = "EN,ES",
                Price = 19.99m
            };

            Assert.Equal(1, garment.GarmentId);
            Assert.Equal("T-Shirt", garment.Name);
            Assert.Equal("Cotton T-Shirt", garment.Description);
            Assert.True(garment.IsActive);
            Assert.Equal("https://example.com/image.png", garment.ImageURL);
            Assert.Equal(50, garment.QuantityAuth);
            Assert.Equal("S,M,L,XL", garment.Sizes);
            Assert.Equal("EN,ES", garment.Languages);
            Assert.Equal(19.99m, garment.Price);
        }

        [Fact]
        public void Garment_Name_Should_Not_Be_Null()
        {
            var garment = new Garment { Name = "Jacket" };

            Assert.NotNull(garment.Name);
            Assert.NotEmpty(garment.Name);
        }

        [Fact]
        public void Garment_Optional_Properties_Can_Be_Null()
        {
            var garment = new Garment { Name = "Hat" };

            Assert.Null(garment.Description);
            Assert.Null(garment.ImageURL);
            Assert.Null(garment.Sizes);
            Assert.Null(garment.Languages);
        }
    }
}
