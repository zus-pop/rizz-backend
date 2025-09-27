using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories
{
    public class OtpCodeRepository : IOtpCodeRepository
    {
        private readonly AuthDbContext _context;

        public OtpCodeRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<OtpCode?> GetValidOtpAsync(string phoneNumber, string code)
        {
            return await _context.OtpCodes
                .FirstOrDefaultAsync(otp => 
                    otp.PhoneNumber == phoneNumber && 
                    otp.Code == code && 
                    !otp.IsUsed && 
                    otp.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<OtpCode?> GetValidOtpByEmailAsync(string email, string code)
        {
            return await _context.OtpCodes
                .FirstOrDefaultAsync(otp => 
                    otp.Email == email && 
                    otp.Code == code && 
                    !otp.IsUsed && 
                    otp.ExpiresAt > DateTime.UtcNow);
        }

        public async Task AddAsync(OtpCode otpCode)
        {
            await _context.OtpCodes.AddAsync(otpCode);
        }

        public async Task MarkAsUsedAsync(int otpCodeId)
        {
            var otpCode = await _context.OtpCodes.FindAsync(otpCodeId);
            if (otpCode != null)
            {
                otpCode.MarkAsUsed();
            }
        }

        public async Task<int> GetRecentOtpCountAsync(string identifier, TimeSpan timeWindow)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(timeWindow);
            return await _context.OtpCodes
                .CountAsync(otp => 
                    (otp.PhoneNumber == identifier || otp.Email == identifier) && 
                    otp.CreatedAt >= cutoffTime);
        }

        public async Task CleanupExpiredOtpAsync()
        {
            var expiredOtps = await _context.OtpCodes
                .Where(otp => otp.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredOtps.Any())
            {
                _context.OtpCodes.RemoveRange(expiredOtps);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}