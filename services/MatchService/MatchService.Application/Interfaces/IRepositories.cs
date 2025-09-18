using MatchService.Domain.Entities;

namespace MatchService.Application.Interfaces
{
    public interface IMatchRepository
    {
        Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Match>> GetUserMatchesAsync(Guid userId, bool activeOnly = true, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<Match?> GetMatchBetweenUsersAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default);
        Task<Match> AddAsync(Match match, CancellationToken cancellationToken = default);
        void Update(Match match);
        void Delete(Match match);
        Task<int> CountUserMatchesAsync(Guid userId, bool activeOnly = true, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> UserHasMatchWithAsync(Guid userId, Guid otherUserId, CancellationToken cancellationToken = default);
    }

    public interface ISwipeRepository
    {
        Task<Swipe?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Swipe>> GetUserSwipesAsync(Guid userId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<Swipe?> GetSwipeBetweenUsersAsync(Guid swiperId, Guid targetUserId, CancellationToken cancellationToken = default);
        Task<bool> HasUserSwipedAsync(Guid swiperId, Guid targetUserId, CancellationToken cancellationToken = default);
        Task<bool> CheckMutualSwipeAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default);
        Task<Swipe> AddAsync(Swipe swipe, CancellationToken cancellationToken = default);
        void Update(Swipe swipe);
        void Delete(Swipe swipe);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }

    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IMatchRepository MatchRepository { get; }
        ISwipeRepository SwipeRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}