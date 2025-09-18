using ModerationService.Domain.Entities;

namespace ModerationService.Application.Interfaces;

public interface IModerationCaseRepository
{
    Task<ModerationCase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ModerationCase>> GetCasesForUserAsync(int targetUserId, CancellationToken cancellationToken = default);
    Task<List<ModerationCase>> GetCasesByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<List<ModerationCase>> GetAssignedCasesAsync(int assignedToUserId, CancellationToken cancellationToken = default);
    Task<List<ModerationCase>> GetCasesByPriorityAsync(string priority, CancellationToken cancellationToken = default);
    Task AddAsync(ModerationCase moderationCase, CancellationToken cancellationToken = default);
    Task UpdateAsync(ModerationCase moderationCase, CancellationToken cancellationToken = default);
}