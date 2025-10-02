using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IJwtProvider
{
    string GenerateJwtToken(AuthUser user);
    string GenerateRefreshToken();
    Task<string?> RefreshTokenAsync(string token, string refreshToken);
    bool ValidateToken(string token);
}
