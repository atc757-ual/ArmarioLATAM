using Xunit;
using AuthService.API.Dtos;

namespace AuthService.Tests.DTOs
{
    public class RegisterRequestTests
    {
        [Fact]
        public void RegisterRequest_ShouldSetProperties()
        {
            var dto = new RegisterRequest
            {
                Email = "user@test.com",
                Password = "pass123",
                Name = "Rafael",
                BP = "BP001"
            };
            Assert.Equal("user@test.com", dto.Email);
            Assert.Equal("pass123", dto.Password);
            Assert.Equal("Rafael", dto.Name);
            Assert.Equal("BP001", dto.BP);
        }
    }
}
