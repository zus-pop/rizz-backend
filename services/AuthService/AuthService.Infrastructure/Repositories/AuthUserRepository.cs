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

    public async Task AddAsync(AuthUser user)
    {
        await _context.AuthUsers.AddAsync(user);
    }

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
