using Microsoft.EntityFrameworkCore;
using ModerationService.API.Models;

namespace ModerationService.API.Data
{
    public class ModerationDbContext : DbContext
    {
        public ModerationDbContext(DbContextOptions<ModerationDbContext> options) : base(options) { }

        public DbSet<Block> Blocks { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Block entity
            modelBuilder.Entity<Block>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BlockerId).IsRequired();
                entity.Property(e => e.BlockedId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                
                // Create unique index to prevent duplicate blocks
                entity.HasIndex(e => new { e.BlockerId, e.BlockedId }).IsUnique();
            });

            // Configure Report entity
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReporterId).IsRequired();
                entity.Property(e => e.ReportedId).IsRequired();
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
