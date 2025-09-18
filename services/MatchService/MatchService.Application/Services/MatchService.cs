using AutoMapper;
using MediatR;
using MatchService.Application.DTOs;
using MatchService.Application.Commands;
using MatchService.Application.Queries;
using MatchService.Application.Interfaces;

namespace MatchService.Application.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public MatchService(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<SwipeDto> CreateSwipeAsync(CreateSwipeDto createSwipeDto, CancellationToken cancellationToken = default)
        {
            var command = new CreateSwipeCommand
            {
                SwiperId = createSwipeDto.SwiperId,
                TargetUserId = createSwipeDto.TargetUserId,
                Direction = createSwipeDto.Direction
            };

            return await _mediator.Send(command, cancellationToken);
        }

        public async Task<MatchDto?> ProcessMatchAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default)
        {
            var command = new ProcessMatchCommand
            {
                User1Id = user1Id,
                User2Id = user2Id
            };

            return await _mediator.Send(command, cancellationToken);
        }

        public async Task<bool> UnmatchAsync(int matchId, Guid userId, CancellationToken cancellationToken = default)
        {
            var command = new UnmatchCommand
            {
                MatchId = matchId,
                UserId = userId
            };

            return await _mediator.Send(command, cancellationToken);
        }

        public async Task<bool> DeleteSwipeAsync(int swipeId, Guid userId, CancellationToken cancellationToken = default)
        {
            var command = new DeleteSwipeCommand
            {
                SwipeId = swipeId,
                UserId = userId
            };

            return await _mediator.Send(command, cancellationToken);
        }

        public async Task<IEnumerable<MatchDto>> GetUserMatchesAsync(Guid userId, bool activeOnly = true, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetUserMatchesQuery
            {
                UserId = userId,
                ActiveOnly = activeOnly,
                Page = page,
                PageSize = pageSize
            };

            return await _mediator.Send(query, cancellationToken);
        }

        public async Task<MatchDto?> GetMatchByIdAsync(int matchId, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            var query = new GetMatchByIdQuery
            {
                MatchId = matchId,
                UserId = userId
            };

            return await _mediator.Send(query, cancellationToken);
        }

        public async Task<IEnumerable<SwipeDto>> GetUserSwipesAsync(Guid userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetUserSwipesQuery
            {
                UserId = userId,
                Page = page,
                PageSize = pageSize
            };

            return await _mediator.Send(query, cancellationToken);
        }

        public async Task<SwipeDto?> GetSwipeByIdAsync(int swipeId, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            var query = new GetSwipeByIdQuery
            {
                SwipeId = swipeId,
                UserId = userId
            };

            return await _mediator.Send(query, cancellationToken);
        }

        public async Task<MatchSummaryDto> GetMatchSummaryAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var query = new GetMatchSummaryQuery
            {
                UserId = userId
            };

            return await _mediator.Send(query, cancellationToken);
        }

        public async Task<bool> CheckMutualSwipeAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default)
        {
            var query = new CheckMutualSwipeQuery
            {
                User1Id = user1Id,
                User2Id = user2Id
            };

            return await _mediator.Send(query, cancellationToken);
        }
    }
}