using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly UserDbContext _context;

        public ProfileRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<Profile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Profile?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        }

        public async Task<Profile> AddAsync(Profile profile, CancellationToken cancellationToken = default)
        {
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync(cancellationToken);
            return profile;
        }

        public async Task UpdateAsync(Profile profile, CancellationToken cancellationToken = default)
        {
            _context.Profiles.Update(profile);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var profile = await _context.Profiles.FindAsync(new object[] { id }, cancellationToken);
            if (profile != null)
            {
                _context.Profiles.Remove(profile);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Profiles.AnyAsync(p => p.UserId == userId, cancellationToken);
        }
    }
}