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

        public async Task<IEnumerable<Profile>> GetProfilesByPreferencesAsync(
            int currentUserId,
            string? interestedInGender,
            int? minAge,
            int? maxAge,
            double? userLatitude,
            double? userLongitude,
            double? maxDistanceKm,
            bool showOnlyVerified,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Users
                .Include(u => u.Profile)
                .Include(u => u.Photos)
                .Where(u => u.Id != currentUserId && u.Profile != null); // Exclude current user and users without profiles

            // Filter by gender preference
            if (!string.IsNullOrEmpty(interestedInGender) && interestedInGender != "both")
            {
                query = query.Where(u => u.Gender == interestedInGender);
            }

            // Filter by age preference
            if (minAge.HasValue || maxAge.HasValue)
            {
                var currentDate = DateTime.UtcNow.Date;
                
                if (minAge.HasValue)
                {
                    var maxBirthDate = currentDate.AddYears(-minAge.Value);
                    query = query.Where(u => u.Birthday.HasValue && u.Birthday <= maxBirthDate);
                }
                
                if (maxAge.HasValue)
                {
                    var minBirthDate = currentDate.AddYears(-maxAge.Value - 1);
                    query = query.Where(u => u.Birthday.HasValue && u.Birthday > minBirthDate);
                }
            }

            // Filter by verification status
            if (showOnlyVerified)
            {
                query = query.Where(u => u.IsVerified);
            }

            // Note: Distance filtering will be done in memory for now due to EF Core translation limitations
            // In production, this should use raw SQL or a stored procedure for better performance
            
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize * 2) // Get more records to account for distance filtering
                .ToListAsync(cancellationToken);

            // Apply distance filtering in memory if location is provided
            if (userLatitude.HasValue && userLongitude.HasValue && maxDistanceKm.HasValue)
            {
                var userLocation = UserService.Domain.ValueObjects.Location.Create(userLatitude.Value, userLongitude.Value);
                var maxDistanceMeters = maxDistanceKm.Value * 1000;

                users = users.Where(u => u.Location != null && 
                                   u.Location.Point.Distance(userLocation.Point) <= maxDistanceMeters)
                            .Take(pageSize)
                            .ToList();
            }

            // Extract profiles from users
            return users.Where(u => u.Profile != null).Select(u => u.Profile!);
        }
    }
}