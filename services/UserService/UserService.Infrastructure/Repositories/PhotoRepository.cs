using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly UserDbContext _context;

        public PhotoRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<Photo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Photos
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Photo>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Photos
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Photo?> GetMainPhotoByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Photos
                .FirstOrDefaultAsync(p => p.UserId == userId && p.IsMainPhoto, cancellationToken);
        }

        public async Task<Photo> AddAsync(Photo photo, CancellationToken cancellationToken = default)
        {
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync(cancellationToken);
            return photo;
        }

        public async Task UpdateAsync(Photo photo, CancellationToken cancellationToken = default)
        {
            _context.Photos.Update(photo);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var photo = await _context.Photos.FindAsync(new object[] { id }, cancellationToken);
            if (photo != null)
            {
                _context.Photos.Remove(photo);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task SetMainPhotoAsync(int userId, int photoId, CancellationToken cancellationToken = default)
        {
            // First, remove main photo status from all user's photos
            var userPhotos = await _context.Photos
                .Where(p => p.UserId == userId)
                .ToListAsync(cancellationToken);

            foreach (var photo in userPhotos)
            {
                photo.RemoveAsMainPhoto();
            }

            // Then set the specified photo as main
            var mainPhoto = userPhotos.FirstOrDefault(p => p.Id == photoId);
            if (mainPhoto != null)
            {
                mainPhoto.SetAsMainPhoto();
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetPhotoCountByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Photos
                .CountAsync(p => p.UserId == userId, cancellationToken);
        }
    }
}