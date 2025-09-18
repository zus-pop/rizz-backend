using MatchService.Application.DTOs;

namespace MatchService.Application.Interfaces
{
    public interface IMatchService
    {
        Task<SwipeDto> CreateSwipeAsync(CreateSwipeDto createSwipeDto, CancellationToken cancellationToken = default);
        Task<MatchDto?> ProcessMatchAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default);
        Task<bool> UnmatchAsync(int matchId, Guid userId, CancellationToken cancellationToken = default);
        Task<bool> DeleteSwipeAsync(int swipeId, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<MatchDto>> GetUserMatchesAsync(Guid userId, bool activeOnly = true, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<MatchDto?> GetMatchByIdAsync(int matchId, Guid? userId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<SwipeDto>> GetUserSwipesAsync(Guid userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<SwipeDto?> GetSwipeByIdAsync(int swipeId, Guid? userId = null, CancellationToken cancellationToken = default);
        Task<MatchSummaryDto> GetMatchSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> CheckMutualSwipeAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default);
    }
}