using AuthService.API.Models;
using Xunit;

namespace AuthService.Tests.Models
{
    public class SizeTests
    {
        [Fact]
        public void Size_Should_Set_And_Get_Properties()
        {
            var size = new Size
            {
                SizeId = 1,
                Code = "M",
                Description = "Medium"
            };

            Assert.Equal(1, size.SizeId);
            Assert.Equal("M", size.Code);
            Assert.Equal("Medium", size.Description);
        }
    }
}
