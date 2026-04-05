using CrochetToysShop.Data;
using Microsoft.EntityFrameworkCore;

namespace CrochetToysShop.Services.Tests.Infrastructure
{
    internal static class TestDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
