using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IJwtProvider
{
    string GenerateJwtToken(AuthUser user);
}
