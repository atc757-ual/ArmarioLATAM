using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AuthService.API.Data;
using AuthService.API.Models;
using AuthService.API.Services;
using System;
using System.Threading.Tasks;

namespace AuthService.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly AuthDbContext _context;
        private readonly AuthService.API.Services.AuthService _service;
        private readonly IConfiguration _config;

        public AuthServiceTests()
        {
            // =========================
            // Configuración InMemory DB
            // =========================
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_AuthService")
                .Options;

            _context = new AuthDbContext(options);

            // =========================
            // Configuración IConfiguration (JWT)
            // =========================
            var inMemorySettings = new System.Collections.Generic.Dictionary<string, string> {
                {"Jwt:Key", "MiClaveSuperSecreta1234567890ABCDEF"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _service = new AuthService.API.Services.AuthService(_context, _config);
        }

        [Fact]
        public async Task Register_ShouldCreateUser_WhenEmailIsNew()
        {
            // Arrange
            string email = "testuser@example.com";
            string password = "Password123!";
            string name = "Rafael";
            string bp = "BP001";

            // Act
            var user = await _service.Register(email, password, name, bp);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(email, user!.Email);
            Assert.Equal(name, user.Name);
            Assert.Equal(bp, user.BP);
            Assert.True(user.IsActive);
        }

        [Fact]
        public async Task Register_ShouldReturnNull_WhenEmailExists()
        {
            // Arrange
            string email = "existing@example.com";
            string password = "Password123!";
            string name = "Rafael";
            string bp = "BP001";

            // Crear usuario existente
            await _service.Register(email, password, name, bp);

            // Act
            var userDuplicate = await _service.Register(email, password, name, bp);

            // Assert
            Assert.Null(userDuplicate);
        }

        [Fact]
        public async Task Login_ShouldReturnJwt_WhenCredentialsAreValid()
        {
            // Arrange
            string email = "loginuser@example.com";
            string password = "Password123!";
            string name = "Rafael";
            string bp = "BP001";

            await _service.Register(email, password, name, bp);

            // Act
            var token = await _service.Login(email, password);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
        }

        [Fact]
        public async Task Login_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Act
            var token = await _service.Login("nonexistent@example.com", "Password123!");

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public async Task Login_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            string email = "wrongpass@example.com";
            string password = "Password123!";
            string wrongPassword = "WrongPassword!";
            string name = "Rafael";
            string bp = "BP001";

            await _service.Register(email, password, name, bp);

            // Act
            var token = await _service.Login(email, wrongPassword);

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public async Task LoginAndGetUser_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            string email = "getuser@example.com";
            string password = "Password123!";
            string name = "Rafael";
            string bp = "BP001";

            var createdUser = await _service.Register(email, password, name, bp);

            // Act
            var user = await _service.LoginAndGetUser(email, password);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(createdUser!.Id, user!.Id);
        }

        [Fact]
        public async Task LoginAndGetUser_ShouldReturnNull_WhenPasswordIsInvalid()
        {
            // Arrange
            string email = "invaliduser@example.com";
            string password = "Password123!";
            string wrongPassword = "WrongPassword!";
            string name = "Rafael";
            string bp = "BP001";

            await _service.Register(email, password, name, bp);

            // Act
            var user = await _service.LoginAndGetUser(email, wrongPassword);

            // Assert
            Assert.Null(user);
        }
    }
}
