using Microsoft.EntityFrameworkCore;
using tivitApi.Data;

namespace tivitApi.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
