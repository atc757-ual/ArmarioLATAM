using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Controllers;
using AuthService.API.Data;
using AuthService.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq; // <-- IMPORTANTE

namespace AuthService.Tests.Controllers
{
    public class KitTypesControllerTests
    {
        private readonly LatamDbContext _context;

        public KitTypesControllerTests()
        {
            var options = new DbContextOptionsBuilder<LatamDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_KitTypes")
                .Options;

            _context = new LatamDbContext(options);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenKitTypesExist()
        {
            // Arrange
            _context.KitTypes.AddRange(
                new KitType { KitTypeId = 1, Name = "Kit A", Description = "Desc A", IsActive = true },
                new KitType { KitTypeId = 2, Name = "Kit B", Description = "Desc B", IsActive = true }
            );
            await _context.SaveChangesAsync();

            var controller = new KitTypesController(_context);

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.NotNull(okResult);

            // Convertir el Value a JObject para acceder a propiedades
            var jObject = JObject.FromObject(okResult.Value);

            // Obtener lista de kitTypes
            var kitTypes = jObject["kitTypes"].ToObject<List<KitType>>();
            Assert.NotNull(kitTypes);
            Assert.Equal(2, kitTypes.Count);
            Assert.Contains(kitTypes, k => k.Name == "Kit A");
            Assert.Contains(kitTypes, k => k.Name == "Kit B");

            // Verificar count
            var count = jObject["count"].Value<int>();
            Assert.Equal(2, count);
        }
    }
}
