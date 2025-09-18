using MediatR;
using MatchService.Application.DTOs;

namespace MatchService.Application.Commands
{
    public class CreateSwipeCommand : IRequest<SwipeDto>
    {
        public Guid SwiperId { get; set; }
        public Guid TargetUserId { get; set; }
        public string Direction { get; set; } = string.Empty;
    }

    public class ProcessMatchCommand : IRequest<MatchDto?>
    {
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
    }

    public class UnmatchCommand : IRequest<bool>
    {
        public int MatchId { get; set; }
        public Guid UserId { get; set; } // For authorization
    }

    public class DeleteSwipeCommand : IRequest<bool>
    {
        public int SwipeId { get; set; }
        public Guid UserId { get; set; } // For authorization
    }
}