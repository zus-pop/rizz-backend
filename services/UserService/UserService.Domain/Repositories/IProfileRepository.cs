using UserService.Domain.Entities;

namespace UserService.Domain.Repositories
{
    public interface IProfileRepository
    {
        Task<Profile?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Profile?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Profile> AddAsync(Profile profile, CancellationToken cancellationToken = default);
        Task UpdateAsync(Profile profile, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Profile>> GetProfilesByPreferencesAsync(
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
            CancellationToken cancellationToken = default);
    }
}