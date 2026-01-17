using AuthService.API.Controllers;
using AuthService.API.Data;
using AuthService.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace AuthService.Tests.Controllers
{
    public class GarmentsControllerTests
    {
        private readonly LatamDbContext _context;
        private readonly GarmentsController _controller;

        public GarmentsControllerTests()
        {
            var options = new DbContextOptionsBuilder<LatamDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Garments")
                .Options;
            _context = new LatamDbContext(options);
            _controller = new GarmentsController(_context);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenGarmentsExist()
        {
            _context.Garments.Add(new Garment { GarmentId = 1, Name = "T-Shirt", IsActive = true });
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotFound_WhenNoGarments()
        {
            var result = await _controller.GetAll() as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }
    }
}
