using Microsoft.EntityFrameworkCore;
using MessagingService.Application.Interfaces;
using MessagingService.Domain.Entities;
using MessagingService.Infrastructure.Data;

namespace MessagingService.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessagingDbContext _context;

        public MessageRepository(MessagingDbContext context)
        {
            _context = context;
        }

        public async Task<Message?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Messages.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<Message>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Messages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Message>> GetMessagesByMatchIdAsync(int matchId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            return await _context.Messages
                .Where(m => m.MatchId == matchId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Message>> GetUnreadMessagesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Messages
                .Where(m => m.ReadAt == null && m.SenderId != userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(int matchId, DateTime? since = null, int limit = 50, CancellationToken cancellationToken = default)
        {
            var query = _context.Messages
                .Where(m => m.MatchId == matchId);

            if (since.HasValue)
            {
                query = query.Where(m => m.CreatedAt > since.Value);
            }

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default)
        {
            var result = await _context.Messages.AddAsync(message, cancellationToken);
            return result.Entity;
        }

        public void Update(Message message)
        {
            _context.Messages.Update(message);
        }

        public void Delete(Message message)
        {
            _context.Messages.Remove(message);
        }
    }
}