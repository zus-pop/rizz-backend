using Microsoft.EntityFrameworkCore;
using MessagingService.API.Models;

namespace MessagingService.API.Data
{
    public class MessagingDbContext : DbContext
    {
        public MessagingDbContext(DbContextOptions<MessagingDbContext> options) : base(options) { }
        public DbSet<Message> Messages { get; set; }
    }
}
