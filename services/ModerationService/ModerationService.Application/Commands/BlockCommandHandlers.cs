using MediatR;
using AutoMapper;
using ModerationService.Application.DTOs;
using ModerationService.Application.Interfaces;
using ModerationService.Domain.Entities;
using ModerationService.Domain.ValueObjects;
using ModerationService.Domain.Events;

namespace ModerationService.Application.Commands;

public class CreateBlockCommandHandler : IRequestHandler<CreateBlockCommand, BlockDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateBlockCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<BlockDto> Handle(CreateBlockCommand request, CancellationToken cancellationToken)
    {
        // Check if an active block already exists
        var existingBlock = await _unitOfWork.Blocks.GetActiveBlockAsync(
            request.BlockerId, request.BlockedUserId, cancellationToken);

        if (existingBlock != null)
        {
            throw new InvalidOperationException("User is already blocked");
        }

        var block = new Block(
            UserId.Create(request.BlockerId),
            UserId.Create(request.BlockedUserId),
            request.Reason
        );

        await _unitOfWork.Blocks.AddAsync(block, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain event
        var domainEvent = new UserBlockedEvent(
            block.Id,
            request.BlockerId,
            request.BlockedUserId,
            request.Reason,
            block.CreatedAt
        );

        await _mediator.Publish(domainEvent, cancellationToken);

        return _mapper.Map<BlockDto>(block);
    }
}

public class RevokeBlockCommandHandler : IRequestHandler<RevokeBlockCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RevokeBlockCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RevokeBlockCommand request, CancellationToken cancellationToken)
    {
        var block = await _unitOfWork.Blocks.GetByIdAsync(request.BlockId, cancellationToken);
        if (block == null)
        {
            return false;
        }

        block.Revoke();
        await _unitOfWork.Blocks.UpdateAsync(block, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class RestoreBlockCommandHandler : IRequestHandler<RestoreBlockCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RestoreBlockCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RestoreBlockCommand request, CancellationToken cancellationToken)
    {
        var block = await _unitOfWork.Blocks.GetByIdAsync(request.BlockId, cancellationToken);
        if (block == null)
        {
            return false;
        }

        block.Restore();
        await _unitOfWork.Blocks.UpdateAsync(block, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}