using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;
using UserService.Domain.Enums;
using DomainLocation = UserService.Domain.ValueObjects.Location;

namespace UserService.Infrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<AIInsight> AIInsights { get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Enable PostGIS extension
            modelBuilder.HasPostgresExtension("postgis");

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                
                // Configure value objects
                entity.Property(u => u.Email)
                    .HasConversion(
                        email => email.Value,
                        value => Email.Create(value))
                    .HasColumnName("Email")
                    .IsRequired();

                entity.Property(u => u.PhoneNumber)
                    .HasConversion(
                        phone => phone.Value,
                        value => PhoneNumber.Create(value))
                    .HasColumnName("PhoneNumber")
                    .IsRequired();

                entity.Property(u => u.Location)
                    .HasConversion(
                        location => location != null ? location.Point : null,
                        point => point != null ? DomainLocation.FromPoint(point) : null)
                    .HasColumnType("geometry (point)");

                // Create indexes after conversion configuration
                entity.HasIndex(nameof(User.Email)).IsUnique();
                entity.HasIndex(nameof(User.PhoneNumber)).IsUnique();
                entity.HasIndex(nameof(User.Location)).HasMethod("GIST"); // spatial index
                
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Gender).IsRequired().HasMaxLength(20);
                entity.Property(u => u.Personality).IsRequired().HasMaxLength(500);
            });

            // Profile entity configuration
            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.UserId).IsUnique();
                
                entity.HasOne<User>()
                    .WithOne(u => u.Profile)
                    .HasForeignKey<Profile>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Basic info fields
                entity.Property(p => p.Bio).HasMaxLength(1000);
                entity.Property(p => p.Voice).HasMaxLength(500); // URL to audio intro
                
                // Enum fields - stored as strings
                entity.Property(p => p.Emotion)
                    .HasConversion<string>();
                entity.Property(p => p.VoiceQuality)
                    .HasConversion<string>();
                entity.Property(p => p.Accent)
                    .HasConversion<string>();
                
                entity.Property(p => p.University).HasMaxLength(200);
                entity.Property(p => p.InterestedIn).HasMaxLength(500);
                entity.Property(p => p.LookingFor).HasMaxLength(500);
                entity.Property(p => p.StudyStyle).HasMaxLength(300);
                entity.Property(p => p.WeekendHobby).HasMaxLength(300);
                entity.Property(p => p.CampusLife).HasMaxLength(500);
                entity.Property(p => p.FuturePlan).HasMaxLength(500);
                entity.Property(p => p.CommunicationPreference).HasMaxLength(50);
                entity.Property(p => p.DealBreakers).HasMaxLength(500);
                entity.Property(p => p.Zodiac).HasMaxLength(50);
                entity.Property(p => p.LoveLanguage).HasMaxLength(50);
            });

            // Photo entity configuration
            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.UserId);
                
                entity.HasOne<User>()
                    .WithMany(u => u.Photos)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.Url).IsRequired().HasMaxLength(500);
                entity.Property(p => p.Description).HasMaxLength(500);
            });

            // Preference entity configuration
            modelBuilder.Entity<Preference>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.UserId).IsUnique();
                
                entity.HasOne<User>()
                    .WithOne(u => u.Preference)
                    .HasForeignKey<Preference>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Enum fields - stored as strings
                entity.Property(p => p.LookingForGender)
                    .HasConversion<string>();
                entity.Property(p => p.Emotion)
                    .HasConversion<string>();
                entity.Property(p => p.VoiceQuality)
                    .HasConversion<string>();
                entity.Property(p => p.Accent)
                    .HasConversion<string>();
                
                // JSON field for interests filter
                entity.Property(p => p.InterestsFilter)
                    .HasColumnType("jsonb");
            });

            // AIInsight entity configuration
            modelBuilder.Entity<AIInsight>(entity =>
            {
                entity.HasKey(ai => ai.Id);
                entity.HasIndex(ai => ai.UserId).IsUnique();
                
                entity.HasOne<User>()
                    .WithOne(u => u.AIInsight)
                    .HasForeignKey<AIInsight>(ai => ai.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ai => ai.PersonalityAnalysis).HasMaxLength(2000);
                entity.Property(ai => ai.CompatibilityFactors).HasMaxLength(2000);
                entity.Property(ai => ai.ImprovementSuggestions).HasMaxLength(2000);
            });

            // DeviceToken entity configuration
            modelBuilder.Entity<DeviceToken>(entity =>
            {
                entity.HasKey(dt => dt.Id);
                entity.HasIndex(dt => dt.Token).IsUnique();
                entity.HasIndex(dt => new { dt.UserId, dt.DeviceType });
                
                entity.HasOne<User>()
                    .WithMany(u => u.DeviceTokens)
                    .HasForeignKey(dt => dt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(dt => dt.Token).IsRequired().HasMaxLength(500);
                entity.Property(dt => dt.DeviceType).IsRequired().HasMaxLength(20);
                entity.Property(dt => dt.DeviceId).HasMaxLength(100);
            });

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetUpdatedAt();
                }
            }
        }
    }
}