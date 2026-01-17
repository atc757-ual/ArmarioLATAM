using AuthService.API.Models;
using Xunit;

namespace AuthService.Tests.Models
{
    public class KitTypeTests
    {
        [Fact]
        public void KitType_Should_Set_And_Get_Properties()
        {
            var kitType = new KitType
            {
                KitTypeId = 1,
                Name = "Premium"
            };

            Assert.Equal(1, kitType.KitTypeId);
            Assert.Equal("Premium", kitType.Name);
        }
    }
}
