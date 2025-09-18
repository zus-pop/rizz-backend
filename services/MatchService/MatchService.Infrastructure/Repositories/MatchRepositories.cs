using Microsoft.EntityFrameworkCore;
using MatchService.Application.Interfaces;
using MatchService.Domain.Entities;
using MatchService.Infrastructure.Data;

namespace MatchService.Infrastructure.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly MatchDbContext _context;

        public MatchRepository(MatchDbContext context)
        {
            _context = context;
        }

        public async Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Match>> GetUserMatchesAsync(
            Guid userId, 
            bool activeOnly = true, 
            int page = 1, 
            int pageSize = 10, 
            CancellationToken cancellationToken = default)
        {
            var query = _context.Matches
                .Where(m => m.User1Id == userId || m.User2Id == userId);

            if (activeOnly)
            {
                query = query.Where(m => m.IsActive);
            }

            return await query
                .OrderByDescending(m => m.MatchedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<Match?> GetMatchBetweenUsersAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default)
        {
            return await _context.Matches
                .FirstOrDefaultAsync(m => 
                    (m.User1Id == user1Id && m.User2Id == user2Id) ||
                    (m.User1Id == user2Id && m.User2Id == user1Id), 
                    cancellationToken);
        }

        public async Task<int> CountUserMatchesAsync(Guid userId, bool activeOnly = true, CancellationToken cancellationToken = default)
        {
            var query = _context.Matches
                .Where(m => m.User1Id == userId || m.User2Id == userId);

            if (activeOnly)
            {
                query = query.Where(m => m.IsActive);
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Match> AddAsync(Match match, CancellationToken cancellationToken = default)
        {
            var entry = await _context.Matches.AddAsync(match, cancellationToken);
            return entry.Entity;
        }

        public void Update(Match match)
        {
            _context.Matches.Update(match);
        }

        public void Delete(Match match)
        {
            match.IsDeleted = true;
            _context.Matches.Update(match);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Matches
                .AnyAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<bool> UserHasMatchWithAsync(Guid userId, Guid otherUserId, CancellationToken cancellationToken = default)
        {
            return await _context.Matches
                .AnyAsync(m => 
                    m.IsActive && 
                    ((m.User1Id == userId && m.User2Id == otherUserId) ||
                     (m.User1Id == otherUserId && m.User2Id == userId)), 
                    cancellationToken);
        }
    }

    public class SwipeRepository : ISwipeRepository
    {
        private readonly MatchDbContext _context;

        public SwipeRepository(MatchDbContext context)
        {
            _context = context;
        }

        public async Task<Swipe?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Swipes
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Swipe>> GetUserSwipesAsync(
            Guid userId, 
            int page = 1, 
            int pageSize = 10, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Swipes
                .Where(s => s.SwiperId == userId)
                .OrderByDescending(s => s.SwipedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<Swipe?> GetSwipeBetweenUsersAsync(Guid swiperId, Guid targetUserId, CancellationToken cancellationToken = default)
        {
            return await _context.Swipes
                .FirstOrDefaultAsync(s => s.SwiperId == swiperId && s.TargetUserId == targetUserId, cancellationToken);
        }

        public async Task<bool> CheckMutualSwipeAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default)
        {
            var swipe1 = await _context.Swipes
                .FirstOrDefaultAsync(s => 
                    s.SwiperId == user1Id && 
                    s.TargetUserId == user2Id && 
                    s.Direction == SwipeDirection.Like, 
                    cancellationToken);

            if (swipe1 == null) return false;

            var swipe2 = await _context.Swipes
                .FirstOrDefaultAsync(s => 
                    s.SwiperId == user2Id && 
                    s.TargetUserId == user1Id && 
                    s.Direction == SwipeDirection.Like, 
                    cancellationToken);

            return swipe2 != null;
        }

        public async Task<Swipe> AddAsync(Swipe swipe, CancellationToken cancellationToken = default)
        {
            var entry = await _context.Swipes.AddAsync(swipe, cancellationToken);
            return entry.Entity;
        }

        public void Update(Swipe swipe)
        {
            _context.Swipes.Update(swipe);
        }

        public void Delete(Swipe swipe)
        {
            swipe.IsDeleted = true;
            _context.Swipes.Update(swipe);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Swipes
                .AnyAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<bool> HasUserSwipedAsync(Guid swiperId, Guid targetUserId, CancellationToken cancellationToken = default)
        {
            return await _context.Swipes
                .AnyAsync(s => s.SwiperId == swiperId && s.TargetUserId == targetUserId, cancellationToken);
        }
    }
}