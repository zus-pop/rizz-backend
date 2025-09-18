using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Domain.ValueObjects;
using UserService.Infrastructure.Data;
using DomainLocation = UserService.Domain.ValueObjects.Location;

namespace UserService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.Preference)
                .Include(u => u.Photos)
                .Include(u => u.DeviceTokens)
                .Include(u => u.AIInsight)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var emailObj = Email.Create(email);
            return await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.Preference)
                .Include(u => u.Photos)
                .Include(u => u.DeviceTokens)
                .Include(u => u.AIInsight)
                .FirstOrDefaultAsync(u => u.Email.Value == emailObj.Value, cancellationToken);
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            var phoneObj = PhoneNumber.Create(phoneNumber);
            return await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.Preference)
                .Include(u => u.Photos)
                .Include(u => u.DeviceTokens)
                .Include(u => u.AIInsight)
                .FirstOrDefaultAsync(u => u.PhoneNumber.Value == phoneObj.Value, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.Photos)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetFilteredAsync(bool? verified = null, string? gender = null, 
                                                             CancellationToken cancellationToken = default)
        {
            var query = _context.Users
                .Include(u => u.Profile)
                .Include(u => u.Photos)
                .AsQueryable();

            if (verified.HasValue)
            {
                query = query.Where(u => u.IsVerified == verified.Value);
            }

            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(u => u.Gender.ToLower() == gender.ToLower());
            }

            return await query
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetNearbyUsersAsync(double latitude, double longitude, double radiusKm, 
                                                               CancellationToken cancellationToken = default)
        {
            var location = DomainLocation.Create(latitude, longitude);
            var radiusMeters = radiusKm * 1000;

            return await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.Photos)
                .Where(u => u.Location != null && 
                           u.Location.Point.Distance(location.Point) <= radiusMeters)
                .OrderBy(u => u.Location!.Point.Distance(location.Point))
                .ToListAsync(cancellationToken);
        }

        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            var emailObj = Email.Create(email);
            var query = _context.Users.Where(u => u.Email.Value == emailObj.Value);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber, int? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            var phoneObj = PhoneNumber.Create(phoneNumber);
            var query = _context.Users.Where(u => u.PhoneNumber.Value == phoneObj.Value);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
    }
}