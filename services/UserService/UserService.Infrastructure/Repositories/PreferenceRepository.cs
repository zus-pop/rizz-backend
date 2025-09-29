using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class PreferenceRepository : IPreferenceRepository
    {
        private readonly UserDbContext _context;

        public PreferenceRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<Preference?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Preferences
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Preference?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Preferences
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        }

        public async Task<Preference> AddAsync(Preference preference, CancellationToken cancellationToken = default)
        {
            _context.Preferences.Add(preference);
            await _context.SaveChangesAsync(cancellationToken);
            return preference;
        }

        public async Task UpdateAsync(Preference preference, CancellationToken cancellationToken = default)
        {
            _context.Preferences.Update(preference);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var preference = await GetByIdAsync(id, cancellationToken);
            if (preference != null)
            {
                _context.Preferences.Remove(preference);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Preferences
                .AnyAsync(p => p.UserId == userId, cancellationToken);
        }
    }
}