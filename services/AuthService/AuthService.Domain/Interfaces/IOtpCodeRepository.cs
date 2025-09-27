using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IOtpCodeRepository
{
    Task<OtpCode?> GetValidOtpAsync(string phoneNumber, string code);
    Task<OtpCode?> GetValidOtpByEmailAsync(string email, string code);
    Task AddAsync(OtpCode otpCode);
    Task MarkAsUsedAsync(int otpCodeId);
    Task<int> GetRecentOtpCountAsync(string identifier, TimeSpan timeWindow);
    Task CleanupExpiredOtpAsync();
    Task SaveChangesAsync();
}
