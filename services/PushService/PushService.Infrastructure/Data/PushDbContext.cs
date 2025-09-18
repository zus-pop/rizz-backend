using Microsoft.EntityFrameworkCore;
using PushService.Domain.Entities;
using PushService.Domain.ValueObjects;

namespace PushService.Infrastructure.Data
{
    public class PushDbContext : DbContext
    {
        public PushDbContext(DbContextOptions<PushDbContext> options) : base(options)
        {
        }

        public DbSet<DeviceToken> DeviceTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DeviceToken configuration
            modelBuilder.Entity<DeviceToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.DeviceType)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.DeviceId)
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.Property(e => e.LastUsedAt);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt);

                // Indexes for performance
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasIndex(e => e.DeviceType);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.UserId, e.DeviceType, e.DeviceId });
            });
        }
    }
}