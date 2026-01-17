using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AuthService.API.Controllers;
using AuthService.API.Data;
using AuthService.API.Models;
using AuthService.API.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthService.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly LatamDbContext _context;
        private readonly AuthDbContext _authContext;
        private readonly OrdersController _controller;
        private readonly Guid _userId;

        public OrdersControllerTests()
        {
            // DB LATAM
            var latamOptions = new DbContextOptionsBuilder<LatamDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new LatamDbContext(latamOptions);

            // DB AUTH
            var authOptions = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _authContext = new AuthDbContext(authOptions);

            // Usuario fake con GUID
            _userId = Guid.NewGuid();
            _authContext.Users.Add(new User
            {
                Id = _userId,
                Email = "test@test.com",
                BP = "BP123",
                Name = "Test User",
                PasswordHash = new byte[32],
                PasswordSalt = new byte[32],
                IsActive = true
            });
            _authContext.SaveChanges();

            _controller = new OrdersController(_context, _authContext);

            // Simular usuario autenticado
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Email, "test@test.com"),
                        new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
                    }, "TestAuth"))
                }
            };
        }

        [Fact]
        public async Task CreateOrder_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            var kitType = new KitType { KitTypeId = 1, Name = "Kit Básico", Description = "Test", IsActive = true };
            var garment = new Garment { GarmentId = 1, Name = "Playera", Price = 100, IsActive = true, Languages = "ES" };
            _context.KitTypes.Add(kitType);
            _context.Garments.Add(garment);
            await _context.SaveChangesAsync();

            var dto = new CreateOrderDto
            {
                KitTypeId = kitType.KitTypeId,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        GarmentId = garment.GarmentId,
                        Quantity = 2,
                        Size = "M"
                    }
                }
            };

            // Act
            var result = await _controller.CreateOrder(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            var result = await _controller.GetOrderById(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
