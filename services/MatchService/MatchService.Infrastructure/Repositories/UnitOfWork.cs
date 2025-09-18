using MatchService.Application.Interfaces;
using MatchService.Infrastructure.Data;
using MatchService.Infrastructure.Repositories;

namespace MatchService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MatchDbContext _context;
        private IMatchRepository? _matchRepository;
        private ISwipeRepository? _swipeRepository;
        private bool _disposed;

        public UnitOfWork(MatchDbContext context)
        {
            _context = context;
        }

        public IMatchRepository MatchRepository
        {
            get
            {
                _matchRepository ??= new MatchRepository(_context);
                return _matchRepository;
            }
        }

        public ISwipeRepository SwipeRepository
        {
            get
            {
                _swipeRepository ??= new SwipeRepository(_context);
                return _swipeRepository;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_context.Database.CurrentTransaction == null)
            {
                await _context.Database.BeginTransactionAsync(cancellationToken);
            }
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = _context.Database.CurrentTransaction;
            if (transaction != null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = _context.Database.CurrentTransaction;
            if (transaction != null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Database.CurrentTransaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.CurrentTransaction.DisposeAsync();
            }
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}