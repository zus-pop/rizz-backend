namespace ModerationService.Application.Interfaces;

public interface IUnitOfWork
{
    IBlockRepository Blocks { get; }
    IReportRepository Reports { get; }
    IModerationCaseRepository ModerationCases { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}