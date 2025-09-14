using Microsoft.EntityFrameworkCore;
using AiInsightsService.API.Models;

namespace AiInsightsService.API.Data
{
    public class AiDbContext : DbContext
    {
        public AiDbContext(DbContextOptions<AiDbContext> options) : base(options) { }

        public DbSet<AiInsight> AiInsights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AiInsight>()
                .HasKey(ai => ai.UserId);
        }
    }
}
