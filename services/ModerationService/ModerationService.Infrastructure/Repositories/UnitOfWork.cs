using Microsoft.EntityFrameworkCore.Storage;
using ModerationService.Application.Interfaces;
using ModerationService.Infrastructure.Data;

namespace ModerationService.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ModerationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IBlockRepository? _blocks;
    private IReportRepository? _reports;
    private IModerationCaseRepository? _moderationCases;

    public UnitOfWork(ModerationDbContext context)
    {
        _context = context;
    }

    public IBlockRepository Blocks => _blocks ??= new BlockRepository(_context);

    public IReportRepository Reports => _reports ??= new ReportRepository(_context);

    public IModerationCaseRepository ModerationCases => _moderationCases ??= new ModerationCaseRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}