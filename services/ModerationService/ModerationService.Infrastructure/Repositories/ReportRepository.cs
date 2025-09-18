using Microsoft.EntityFrameworkCore;
using ModerationService.Application.Interfaces;
using ModerationService.Domain.Entities;
using ModerationService.Infrastructure.Data;

namespace ModerationService.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ModerationDbContext _context;

    public ReportRepository(ModerationDbContext context)
    {
        _context = context;
    }

    public async Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<Report>> GetReportsForUserAsync(int reportedUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Where(r => r.ReportedUserId.Value == reportedUserId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Report>> GetReportsByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        if (Enum.TryParse<ReportStatus>(status, true, out var reportStatus))
        {
            return await _context.Reports
                .Where(r => r.Status == reportStatus)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        return new List<Report>();
    }

    public async Task<List<Report>> GetPendingReportsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Where(r => r.Status == ReportStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Report report, CancellationToken cancellationToken = default)
    {
        await _context.Reports.AddAsync(report, cancellationToken);
    }

    public Task UpdateAsync(Report report, CancellationToken cancellationToken = default)
    {
        _context.Reports.Update(report);
        return Task.CompletedTask;
    }

    public async Task<int> GetActiveReportsCountForUserAsync(int reportedUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .CountAsync(r => 
                r.ReportedUserId.Value == reportedUserId && 
                (r.Status == ReportStatus.Pending || r.Status == ReportStatus.UnderReview), 
                cancellationToken);
    }
}