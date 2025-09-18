using Microsoft.EntityFrameworkCore;
using MessagingService.Domain.Entities;
using MessagingService.Infrastructure.Configurations;

namespace MessagingService.Infrastructure.Data
{
    public class MessagingDbContext : DbContext
    {
        public MessagingDbContext(DbContextOptions<MessagingDbContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MessageConfiguration());
        }
    }
}