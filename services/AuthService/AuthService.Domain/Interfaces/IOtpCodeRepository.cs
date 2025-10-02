using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IOtpCodeRepository
{
    Task<OtpCode?> GetValidOtpAsync(string phoneNumber, string code);
    Task AddAsync(OtpCode otpCode);
    Task SaveChangesAsync();
}
