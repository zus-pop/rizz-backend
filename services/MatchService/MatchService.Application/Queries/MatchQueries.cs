using MediatR;
using MatchService.Application.DTOs;

namespace MatchService.Application.Queries
{
    public class GetUserMatchesQuery : IRequest<IEnumerable<MatchDto>>
    {
        public Guid UserId { get; set; }
        public bool ActiveOnly { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class GetMatchByIdQuery : IRequest<MatchDto?>
    {
        public int MatchId { get; set; }
        public Guid? UserId { get; set; } // For authorization
    }

    public class GetUserSwipesQuery : IRequest<IEnumerable<SwipeDto>>
    {
        public Guid UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class GetSwipeByIdQuery : IRequest<SwipeDto?>
    {
        public int SwipeId { get; set; }
        public Guid? UserId { get; set; } // For authorization
    }

    public class GetMatchSummaryQuery : IRequest<MatchSummaryDto>
    {
        public Guid UserId { get; set; }
    }

    public class CheckMutualSwipeQuery : IRequest<bool>
    {
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
    }
}