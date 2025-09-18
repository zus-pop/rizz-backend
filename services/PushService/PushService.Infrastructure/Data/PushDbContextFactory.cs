using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PushService.Infrastructure.Data
{
    public class PushDbContextFactory : IDesignTimeDbContextFactory<PushDbContext>
    {
        public PushDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PushDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=push_db;Username=datingapp;Password=password123");

            return new PushDbContext(optionsBuilder.Options);
        }
    }
}