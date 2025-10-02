using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetByEmailAsync(string email);
    Task<AuthUser?> GetByIdAsync(int id);
    Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber);
    Task AddAsync(AuthUser user);
    Task UpdateAsync(AuthUser user);
    Task SaveChangesAsync();
}
