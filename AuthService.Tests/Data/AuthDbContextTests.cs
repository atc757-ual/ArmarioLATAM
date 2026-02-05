using Xunit;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;
using AuthService.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Tests.Data
{
    public class AuthDbContextTests
    {
        private DbContextOptions<AuthDbContext> GetInMemoryOptions()
        {
            return new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task AddUser_ShouldSaveUserSuccessfully()
        {
            // Arrange
            var options = GetInMemoryOptions();

            await using var context = new AuthDbContext(options);

            var user = new User
            {
                Id = Guid.NewGuid(),
                BP = "BP001",
                Name = "Test User",
                Email = "testuser@example.com",
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 4, 5, 6 },
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Assert
            var savedUser = context.Users.FirstOrDefault(u => u.Email == "testuser@example.com");
            Assert.NotNull(savedUser);
            Assert.Equal("BP001", savedUser.BP);
            Assert.Equal("Test User", savedUser.Name);
            Assert.True(savedUser.IsActive);
        }

        [Fact]
        public async Task AddUser_WithoutRequiredFields_ShouldFail()
        {
            // Arrange
            var options = GetInMemoryOptions();

            await using var context = new AuthDbContext(options);

            var user = new User
            {
                Id = Guid.NewGuid()
                // No BP, Name, Email, etc.
            };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            });
        }
    }
}
