using AutoMapper;
using MediatR;
using MatchService.Application.DTOs;
using MatchService.Application.Interfaces;
using MatchService.Application.Queries;

namespace MatchService.Application.Queries
{
    public class GetUserMatchesQueryHandler : IRequestHandler<GetUserMatchesQuery, IEnumerable<MatchDto>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public GetUserMatchesQueryHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MatchDto>> Handle(GetUserMatchesQuery request, CancellationToken cancellationToken)
        {
            var matches = await _matchRepository.GetUserMatchesAsync(
                request.UserId, 
                request.ActiveOnly, 
                request.Page, 
                request.PageSize, 
                cancellationToken);

            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }
    }

    public class GetMatchByIdQueryHandler : IRequestHandler<GetMatchByIdQuery, MatchDto?>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public GetMatchByIdQueryHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<MatchDto?> Handle(GetMatchByIdQuery request, CancellationToken cancellationToken)
        {
            var match = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);
            if (match == null || (request.UserId.HasValue && !match.InvolvesUser(request.UserId.Value)))
            {
                return null;
            }

            return _mapper.Map<MatchDto>(match);
        }
    }

    public class GetUserSwipesQueryHandler : IRequestHandler<GetUserSwipesQuery, IEnumerable<SwipeDto>>
    {
        private readonly ISwipeRepository _swipeRepository;
        private readonly IMapper _mapper;

        public GetUserSwipesQueryHandler(ISwipeRepository swipeRepository, IMapper mapper)
        {
            _swipeRepository = swipeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SwipeDto>> Handle(GetUserSwipesQuery request, CancellationToken cancellationToken)
        {
            var swipes = await _swipeRepository.GetUserSwipesAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
            return _mapper.Map<IEnumerable<SwipeDto>>(swipes);
        }
    }

    public class GetSwipeByIdQueryHandler : IRequestHandler<GetSwipeByIdQuery, SwipeDto?>
    {
        private readonly ISwipeRepository _swipeRepository;
        private readonly IMapper _mapper;

        public GetSwipeByIdQueryHandler(ISwipeRepository swipeRepository, IMapper mapper)
        {
            _swipeRepository = swipeRepository;
            _mapper = mapper;
        }

        public async Task<SwipeDto?> Handle(GetSwipeByIdQuery request, CancellationToken cancellationToken)
        {
            var swipe = await _swipeRepository.GetByIdAsync(request.SwipeId, cancellationToken);
            if (swipe == null || (request.UserId.HasValue && swipe.SwiperId != request.UserId.Value))
            {
                return null;
            }

            return _mapper.Map<SwipeDto>(swipe);
        }
    }

    public class GetMatchSummaryQueryHandler : IRequestHandler<GetMatchSummaryQuery, MatchSummaryDto>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public GetMatchSummaryQueryHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<MatchSummaryDto> Handle(GetMatchSummaryQuery request, CancellationToken cancellationToken)
        {
            var totalMatches = await _matchRepository.CountUserMatchesAsync(request.UserId, false, cancellationToken);
            var activeMatches = await _matchRepository.CountUserMatchesAsync(request.UserId, true, cancellationToken);
            var recentMatches = await _matchRepository.GetUserMatchesAsync(request.UserId, true, 1, 10, cancellationToken);

            return new MatchSummaryDto
            {
                TotalMatches = totalMatches,
                ActiveMatches = activeMatches,
                RecentMatches = _mapper.Map<IEnumerable<MatchDto>>(recentMatches)
            };
        }
    }

    public class CheckMutualSwipeQueryHandler : IRequestHandler<CheckMutualSwipeQuery, bool>
    {
        private readonly ISwipeRepository _swipeRepository;

        public CheckMutualSwipeQueryHandler(ISwipeRepository swipeRepository)
        {
            _swipeRepository = swipeRepository;
        }

        public async Task<bool> Handle(CheckMutualSwipeQuery request, CancellationToken cancellationToken)
        {
            return await _swipeRepository.CheckMutualSwipeAsync(request.User1Id, request.User2Id, cancellationToken);
        }
    }
}