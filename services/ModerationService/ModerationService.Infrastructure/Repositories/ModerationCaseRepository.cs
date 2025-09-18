using Microsoft.EntityFrameworkCore;
using ModerationService.Application.Interfaces;
using ModerationService.Domain.Entities;
using ModerationService.Infrastructure.Data;

namespace ModerationService.Infrastructure.Repositories;

public class ModerationCaseRepository : IModerationCaseRepository
{
    private readonly ModerationDbContext _context;

    public ModerationCaseRepository(ModerationDbContext context)
    {
        _context = context;
    }

    public async Task<ModerationCase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ModerationCases
            .FirstOrDefaultAsync(mc => mc.Id == id, cancellationToken);
    }

    public async Task<List<ModerationCase>> GetCasesForUserAsync(int targetUserId, CancellationToken cancellationToken = default)
    {
        return await _context.ModerationCases
            .Where(mc => mc.TargetUserId.Value == targetUserId)
            .OrderByDescending(mc => mc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ModerationCase>> GetCasesByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        if (Enum.TryParse<ModerationCaseStatus>(status, true, out var caseStatus))
        {
            return await _context.ModerationCases
                .Where(mc => mc.Status == caseStatus)
                .OrderByDescending(mc => mc.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        return new List<ModerationCase>();
    }

    public async Task<List<ModerationCase>> GetAssignedCasesAsync(int assignedToUserId, CancellationToken cancellationToken = default)
    {
        return await _context.ModerationCases
            .Where(mc => mc.AssignedTo != null && mc.AssignedTo.Value == assignedToUserId)
            .OrderByDescending(mc => mc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ModerationCase>> GetCasesByPriorityAsync(string priority, CancellationToken cancellationToken = default)
    {
        if (Enum.TryParse<ModerationCasePriority>(priority, true, out var casePriority))
        {
            return await _context.ModerationCases
                .Where(mc => mc.Priority == casePriority)
                .OrderByDescending(mc => mc.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        return new List<ModerationCase>();
    }

    public async Task AddAsync(ModerationCase moderationCase, CancellationToken cancellationToken = default)
    {
        await _context.ModerationCases.AddAsync(moderationCase, cancellationToken);
    }

    public Task UpdateAsync(ModerationCase moderationCase, CancellationToken cancellationToken = default)
    {
        _context.ModerationCases.Update(moderationCase);
        return Task.CompletedTask;
    }
}