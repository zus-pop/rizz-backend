using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using UserService.API.Models;

namespace UserService.API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<AIInsight> AIInsights { get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Enable PostGIS extension
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Location).HasColumnType("geometry (point)");
                entity.HasIndex(u => u.Location).HasMethod("GIST"); // spatial index
            });

            modelBuilder.Entity<Profile>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Profile>(p => p.UserId);

            modelBuilder.Entity<Photo>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Preference>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Preference>(p => p.UserId);

            modelBuilder.Entity<AIInsight>()
                .HasKey(ai => ai.UserId); // 1:1

            modelBuilder.Entity<DeviceToken>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(dt => dt.UserId);
        }
    }
}
