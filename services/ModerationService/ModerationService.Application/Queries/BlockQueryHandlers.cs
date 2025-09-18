using MediatR;
using AutoMapper;
using ModerationService.Application.DTOs;
using ModerationService.Application.Interfaces;

namespace ModerationService.Application.Queries;

public class GetBlockByIdQueryHandler : IRequestHandler<GetBlockByIdQuery, BlockDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBlockByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BlockDto?> Handle(GetBlockByIdQuery request, CancellationToken cancellationToken)
    {
        var block = await _unitOfWork.Blocks.GetByIdAsync(request.BlockId, cancellationToken);
        return block != null ? _mapper.Map<BlockDto>(block) : null;
    }
}

public class GetUserBlocksQueryHandler : IRequestHandler<GetUserBlocksQuery, List<BlockDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserBlocksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<BlockDto>> Handle(GetUserBlocksQuery request, CancellationToken cancellationToken)
    {
        var blocks = await _unitOfWork.Blocks.GetBlocksByUserAsync(request.UserId, cancellationToken);
        return _mapper.Map<List<BlockDto>>(blocks);
    }
}

public class GetBlocksForUserQueryHandler : IRequestHandler<GetBlocksForUserQuery, List<BlockDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBlocksForUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<BlockDto>> Handle(GetBlocksForUserQuery request, CancellationToken cancellationToken)
    {
        var blocks = await _unitOfWork.Blocks.GetBlocksForUserAsync(request.BlockedUserId, cancellationToken);
        return _mapper.Map<List<BlockDto>>(blocks);
    }
}

public class CheckActiveBlockQueryHandler : IRequestHandler<CheckActiveBlockQuery, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckActiveBlockQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CheckActiveBlockQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Blocks.ExistsActiveBlockAsync(
            request.BlockerId, request.BlockedUserId, cancellationToken);
    }
}