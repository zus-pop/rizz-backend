using UserService.Domain.Entities;

namespace UserService.Domain.Repositories
{
    public interface IPhotoRepository
    {
        Task<Photo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Photo>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Photo?> GetMainPhotoByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Photo> AddAsync(Photo photo, CancellationToken cancellationToken = default);
        Task UpdateAsync(Photo photo, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task SetMainPhotoAsync(int userId, int photoId, CancellationToken cancellationToken = default);
        Task<int> GetPhotoCountByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}