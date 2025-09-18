using ModerationService.Domain.Entities;

namespace ModerationService.Application.Interfaces;

public interface IBlockRepository
{
    Task<Block?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Block?> GetActiveBlockAsync(int blockerId, int blockedUserId, CancellationToken cancellationToken = default);
    Task<List<Block>> GetBlocksByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<Block>> GetBlocksForUserAsync(int blockedUserId, CancellationToken cancellationToken = default);
    Task AddAsync(Block block, CancellationToken cancellationToken = default);
    Task UpdateAsync(Block block, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveBlockAsync(int blockerId, int blockedUserId, CancellationToken cancellationToken = default);
}