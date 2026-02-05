using Xunit;
using AuthService.API.Dtos;

namespace AuthService.Tests.DTOs
{
    public class LoginRequestTests
    {
        [Fact]
        public void LoginRequest_ShouldSetProperties()
        {
            var dto = new LoginRequest
            {
                Email = "test@example.com",
                Password = "123456"
            };
            Assert.Equal("test@example.com", dto.Email);
            Assert.Equal("123456", dto.Password);
        }
    }
}
