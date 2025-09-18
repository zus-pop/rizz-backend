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
    }
}