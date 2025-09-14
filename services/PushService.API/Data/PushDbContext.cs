using Microsoft.EntityFrameworkCore;
using PushService.API.Models;

namespace PushService.API.Data
{
    public class PushDbContext : DbContext
    {
        public PushDbContext(DbContextOptions<PushDbContext> options) : base(options) { }

        public DbSet<DeviceToken> DeviceTokens { get; set; }
    }
}
