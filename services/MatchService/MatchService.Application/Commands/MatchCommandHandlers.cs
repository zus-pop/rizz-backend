using AutoMapper;
using MediatR;
using MatchService.Application.Commands;
using MatchService.Application.DTOs;
using MatchService.Application.Interfaces;
using MatchService.Domain.Entities;
using MatchService.Domain.Events;

namespace MatchService.Application.Commands
{
    public class CreateSwipeCommandHandler : IRequestHandler<CreateSwipeCommand, SwipeDto>
    {
        private readonly ISwipeRepository _swipeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSwipeCommandHandler(ISwipeRepository swipeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _swipeRepository = swipeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SwipeDto> Handle(CreateSwipeCommand request, CancellationToken cancellationToken)
        {
            // Check if swipe already exists
            var existingSwipe = await _swipeRepository.GetSwipeBetweenUsersAsync(request.SwiperId, request.TargetUserId, cancellationToken);
            if (existingSwipe != null)
            {
                throw new InvalidOperationException("User has already swiped on this profile");
            }

            // Parse direction
            if (!Enum.TryParse<SwipeDirection>(request.Direction, true, out var direction))
            {
                throw new ArgumentException("Invalid swipe direction");
            }

            var swipe = new Swipe
            {
                SwiperId = request.SwiperId,
                TargetUserId = request.TargetUserId,
                Direction = direction
            };

            if (!swipe.IsValidSwipe())
            {
                throw new ArgumentException("Invalid swipe data");
            }

            var result = await _swipeRepository.AddAsync(swipe, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<SwipeDto>(result);
        }
    }

    public class ProcessMatchCommandHandler : IRequestHandler<ProcessMatchCommand, MatchDto?>
    {
        private readonly ISwipeRepository _swipeRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProcessMatchCommandHandler(
            ISwipeRepository swipeRepository, 
            IMatchRepository matchRepository, 
            IUnitOfWork unitOfWork, 
            IMapper mapper)
        {
            _swipeRepository = swipeRepository;
            _matchRepository = matchRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MatchDto?> Handle(ProcessMatchCommand request, CancellationToken cancellationToken)
        {
            // Check if users have mutual likes
            var hasMutualSwipe = await _swipeRepository.CheckMutualSwipeAsync(request.User1Id, request.User2Id, cancellationToken);
            if (!hasMutualSwipe)
            {
                return null; // No mutual like, no match created
            }

            // Check if match already exists
            var existingMatch = await _matchRepository.GetMatchBetweenUsersAsync(request.User1Id, request.User2Id, cancellationToken);
            if (existingMatch != null)
            {
                return _mapper.Map<MatchDto>(existingMatch); // Return existing match
            }

            // Create new match
            var match = new Match
            {
                User1Id = request.User1Id,
                User2Id = request.User2Id
            };

            if (!match.IsValidMatch())
            {
                throw new ArgumentException("Invalid match data");
            }

            var result = await _matchRepository.AddAsync(match, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<MatchDto>(result);
        }
    }

    public class UnmatchCommandHandler : IRequestHandler<UnmatchCommand, bool>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnmatchCommandHandler(IMatchRepository matchRepository, IUnitOfWork unitOfWork)
        {
            _matchRepository = matchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UnmatchCommand request, CancellationToken cancellationToken)
        {
            var match = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);
            if (match == null || !match.InvolvesUser(request.UserId))
            {
                return false;
            }

            match.Unmatch(request.UserId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class DeleteSwipeCommandHandler : IRequestHandler<DeleteSwipeCommand, bool>
    {
        private readonly ISwipeRepository _swipeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSwipeCommandHandler(ISwipeRepository swipeRepository, IUnitOfWork unitOfWork)
        {
            _swipeRepository = swipeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSwipeCommand request, CancellationToken cancellationToken)
        {
            var swipe = await _swipeRepository.GetByIdAsync(request.SwipeId, cancellationToken);
            if (swipe == null || swipe.SwiperId != request.UserId)
            {
                return false;
            }

            _swipeRepository.Delete(swipe);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}