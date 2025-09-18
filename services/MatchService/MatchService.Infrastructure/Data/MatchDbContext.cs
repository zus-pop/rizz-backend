using Microsoft.EntityFrameworkCore;
using MatchService.Domain.Entities;
using MatchService.Infrastructure.Configurations;
using Common.Domain;

namespace MatchService.Infrastructure.Data
{
    public class MatchDbContext : DbContext
    {
        public MatchDbContext(DbContextOptions<MatchDbContext> options) : base(options)
        {
        }

        public DbSet<Match> Matches { get; set; }
        public DbSet<Swipe> Swipes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new MatchConfiguration());
            modelBuilder.ApplyConfiguration(new SwipeConfiguration());

            // Configure schema
            modelBuilder.HasDefaultSchema("match_service");

            // Add database indexes for performance
            modelBuilder.Entity<Match>()
                .HasIndex(m => new { m.User1Id, m.User2Id })
                .IsUnique()
                .HasDatabaseName("IX_Matches_Users");

            modelBuilder.Entity<Match>()
                .HasIndex(m => m.IsActive)
                .HasDatabaseName("IX_Matches_IsActive");

            modelBuilder.Entity<Swipe>()
                .HasIndex(s => new { s.SwiperId, s.TargetUserId })
                .IsUnique()
                .HasDatabaseName("IX_Swipes_Users");

            modelBuilder.Entity<Swipe>()
                .HasIndex(s => s.SwiperId)
                .HasDatabaseName("IX_Swipes_SwiperId");

            modelBuilder.Entity<Swipe>()
                .HasIndex(s => s.TargetUserId)
                .HasDatabaseName("IX_Swipes_TargetUserId");

            modelBuilder.Entity<Swipe>()
                .HasIndex(s => s.Direction)
                .HasDatabaseName("IX_Swipes_Direction");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }

                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}