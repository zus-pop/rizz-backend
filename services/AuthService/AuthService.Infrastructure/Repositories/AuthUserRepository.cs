using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class AuthUserRepository : IAuthUserRepository
{
    private readonly AuthDbContext _context;
    public AuthUserRepository(AuthDbContext context) => _context = context;

    public async Task<AuthUser?> GetByEmailAsync(string email)
        => await _context.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<AuthUser?> GetByIdAsync(int id)
        => await _context.AuthUsers.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<AuthUser?> GetByPhoneNumberAsync(string phoneNumber)
        => await _context.AuthUsers.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

    public async Task AddAsync(AuthUser user)
    {
        await _context.AuthUsers.AddAsync(user);
    }

    public async Task UpdateAsync(AuthUser user)
    {
        _context.AuthUsers.Update(user);
    }

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
