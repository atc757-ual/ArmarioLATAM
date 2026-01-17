using AuthService.API.Models;
using Xunit;

namespace AuthService.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void User_Should_Set_And_Get_Properties()
        {
            var userId = Guid.NewGuid();
            var passwordHash = new byte[] { 1, 2, 3 };
            var passwordSalt = new byte[] { 4, 5, 6 };
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = "Rafael",
                BP = "123456",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            Assert.Equal(userId, user.Id);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal(passwordHash, user.PasswordHash);
            Assert.Equal(passwordSalt, user.PasswordSalt);
            Assert.Equal("Rafael", user.Name);
            Assert.Equal("123456", user.BP);
            Assert.True(user.IsActive);
            Assert.True(user.CreatedAt <= DateTime.Now);
        }

        [Fact]
        public void User_Email_Should_Not_Be_Null_Or_Empty()
        {
            var user = new User
            {
                Email = "someone@example.com",
                PasswordHash = new byte[] { 1 },
                PasswordSalt = new byte[] { 2 },
                Name = "Name",
                BP = "BP01"
            };

            Assert.False(string.IsNullOrEmpty(user.Email));
        }

        [Fact]
        public void User_Name_And_BP_Should_Not_Be_Null_Or_Empty()
        {
            var user = new User
            {
                Name = "Rafael",
                BP = "123456",
                Email = "email@test.com",
                PasswordHash = new byte[] { 1 },
                PasswordSalt = new byte[] { 2 }
            };

            Assert.False(string.IsNullOrEmpty(user.Name));
            Assert.False(string.IsNullOrEmpty(user.BP));
        }
    }
}
