using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    
    public DbSet<AuthUser> AuthUsers { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // AuthUser configuration
        modelBuilder.Entity<AuthUser>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.PhoneNumber).IsUnique();
            
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.RefreshToken).HasMaxLength(500);
        });

        // OtpCode configuration
        modelBuilder.Entity<OtpCode>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.HasIndex(o => o.PhoneNumber);
            entity.HasIndex(o => o.Email);
            entity.HasIndex(o => o.CreatedAt);
            
            entity.Property(o => o.PhoneNumber).HasMaxLength(20);
            entity.Property(o => o.Email).HasMaxLength(255);
            entity.Property(o => o.Code).IsRequired().HasMaxLength(10);
            entity.Property(o => o.Purpose).IsRequired().HasMaxLength(50);
        });

        base.OnModelCreating(modelBuilder);
    }
}
