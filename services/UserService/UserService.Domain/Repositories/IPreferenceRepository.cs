using UserService.Domain.Entities;

namespace UserService.Domain.Repositories
{
    public interface IPreferenceRepository
    {
        Task<Preference?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Preference?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Preference> AddAsync(Preference preference, CancellationToken cancellationToken = default);
        Task UpdateAsync(Preference preference, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int userId, CancellationToken cancellationToken = default);
    }
}