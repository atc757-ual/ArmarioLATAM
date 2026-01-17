using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;

namespace AuthService.Tests.Helpers;

public static class TestDbContextFactory
{
    public static LatamDbContext CreateLatamContext()
    {
        var options = new DbContextOptionsBuilder<LatamDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LatamDbContext(options);
    }

    public static AuthDbContext CreateAuthContext()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AuthDbContext(options);
    }
}
