using MessagingService.Domain.Entities;

namespace MessagingService.Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Message>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Message>> GetMessagesByMatchIdAsync(int matchId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<IEnumerable<Message>> GetUnreadMessagesByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Message>> GetConversationAsync(int matchId, DateTime? since = null, int limit = 50, CancellationToken cancellationToken = default);
        Task<IEnumerable<Message>> GetVoiceMessagesByMatchIdAsync(int matchId, CancellationToken cancellationToken = default);
        Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default);
        Task<Message> CreateAsync(Message message, CancellationToken cancellationToken = default);
        void Update(Message message);
        void Delete(Message message);
    }

    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}