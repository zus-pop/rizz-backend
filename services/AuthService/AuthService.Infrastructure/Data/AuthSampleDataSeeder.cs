using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using BCrypt.Net;

namespace AuthService.Infrastructure.Data
{
    public static class AuthSampleDataSeeder
    {
        public static async Task SeedSampleDataAsync(AuthDbContext context)
        {
            // Check if data already exists
            if (await context.AuthUsers.AnyAsync())
                return;

            // Create sample users
            var users = new List<AuthUser>
            {
                new AuthUser
                {
                    Email = "john.doe@example.com",
                    PhoneNumber = "+1234567890",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    IsVerified = true,
                    IsEmailVerified = true,
                    IsPhoneVerified = true,
                    VerifiedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    FailedLoginAttempts = 0
                },
                new AuthUser
                {
                    Email = "jane.smith@example.com",
                    PhoneNumber = "+1234567891",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    IsVerified = false,
                    IsEmailVerified = false,
                    IsPhoneVerified = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    FailedLoginAttempts = 0
                },
                new AuthUser
                {
                    Email = "mike.johnson@example.com",
                    PhoneNumber = "+1234567892",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    IsVerified = true,
                    IsEmailVerified = true,
                    IsPhoneVerified = true,
                    VerifiedAt = DateTime.UtcNow.AddDays(-20),
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    FailedLoginAttempts = 0
                },
                new AuthUser
                {
                    Email = "sarah.williams@example.com",
                    PhoneNumber = "+1234567893",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    IsVerified = true,
                    IsEmailVerified = true,
                    IsPhoneVerified = true,
                    VerifiedAt = DateTime.UtcNow.AddDays(-15),
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    FailedLoginAttempts = 0
                },
                new AuthUser
                {
                    Email = "david.brown@example.com",
                    PhoneNumber = "+1234567894",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    IsVerified = false,
                    IsEmailVerified = true,
                    IsPhoneVerified = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    FailedLoginAttempts = 0
                },
                new AuthUser
                {
                    Email = "emily.davis@example.com",
                    PhoneNumber = "+1234567895",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    IsVerified = true,
                    IsEmailVerified = true,
                    IsPhoneVerified = true,
                    VerifiedAt = DateTime.UtcNow.AddDays(-5),
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    FailedLoginAttempts = 0
                },
                new AuthUser
                {
                    Email = "admin@example.com",
                    PhoneNumber = "+1234567896",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"),
                    IsVerified = true,
                    IsEmailVerified = true,
                    IsPhoneVerified = true,
                    VerifiedAt = DateTime.UtcNow.AddDays(-100),
                    CreatedAt = DateTime.UtcNow.AddDays(-100),
                    FailedLoginAttempts = 0
                },
                new AuthUser
                {
                    Email = "test@example.com",
                    PhoneNumber = "+1234567897",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPassword123!"),
                    IsVerified = true,
                    IsEmailVerified = true,
                    IsPhoneVerified = true,
                    VerifiedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    FailedLoginAttempts = 0
                }
            };

            context.AuthUsers.AddRange(users);
            await context.SaveChangesAsync();

            // Create some sample OTP codes for testing (expired ones)
            var otpCodes = new List<OtpCode>
            {
                OtpCode.CreateForEmail("jane.smith@example.com", "123456", "email_verification", 10),
                OtpCode.CreateForPhone("+1234567891", "654321", "phone_verification", 10),
                OtpCode.CreateForEmail("david.brown@example.com", "789012", "email_verification", 10),
                OtpCode.CreateForPhone("+1234567894", "210987", "phone_verification", 10)
            };

            // Set these OTPs as expired for testing purposes
            foreach (var otp in otpCodes)
            {
                otp.CreatedAt = DateTime.UtcNow.AddMinutes(-15);
                otp.ExpiresAt = DateTime.UtcNow.AddMinutes(-5);
            }

            context.OtpCodes.AddRange(otpCodes);
            await context.SaveChangesAsync();
        }
    }
}
