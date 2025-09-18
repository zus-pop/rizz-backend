using ModerationService.Domain.Entities;

namespace ModerationService.Application.Interfaces;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Report>> GetReportsForUserAsync(int reportedUserId, CancellationToken cancellationToken = default);
    Task<List<Report>> GetReportsByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<List<Report>> GetPendingReportsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Report report, CancellationToken cancellationToken = default);
    Task UpdateAsync(Report report, CancellationToken cancellationToken = default);
    Task<int> GetActiveReportsCountForUserAsync(int reportedUserId, CancellationToken cancellationToken = default);
}