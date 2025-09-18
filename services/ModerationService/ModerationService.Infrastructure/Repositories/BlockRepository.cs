using Microsoft.EntityFrameworkCore;
using ModerationService.Application.Interfaces;
using ModerationService.Domain.Entities;
using ModerationService.Infrastructure.Data;

namespace ModerationService.Infrastructure.Repositories;

public class BlockRepository : IBlockRepository
{
    private readonly ModerationDbContext _context;

    public BlockRepository(ModerationDbContext context)
    {
        _context = context;
    }

    public async Task<Block?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Blocks
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Block?> GetActiveBlockAsync(int blockerId, int blockedUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Blocks
            .FirstOrDefaultAsync(b => 
                b.BlockerId.Value == blockerId && 
                b.BlockedUserId.Value == blockedUserId && 
                b.IsActive, 
                cancellationToken);
    }

    public async Task<List<Block>> GetBlocksByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Blocks
            .Where(b => b.BlockerId.Value == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Block>> GetBlocksForUserAsync(int blockedUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Blocks
            .Where(b => b.BlockedUserId.Value == blockedUserId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Block block, CancellationToken cancellationToken = default)
    {
        await _context.Blocks.AddAsync(block, cancellationToken);
    }

    public Task UpdateAsync(Block block, CancellationToken cancellationToken = default)
    {
        _context.Blocks.Update(block);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsActiveBlockAsync(int blockerId, int blockedUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Blocks
            .AnyAsync(b => 
                b.BlockerId.Value == blockerId && 
                b.BlockedUserId.Value == blockedUserId && 
                b.IsActive, 
                cancellationToken);
    }
}