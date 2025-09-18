using MediatR;
using ModerationService.Application.DTOs;

namespace ModerationService.Application.Commands;

public record CreateBlockCommand(
    int BlockerId,
    int BlockedUserId,
    string? Reason
) : IRequest<BlockDto>;

public record RevokeBlockCommand(
    Guid BlockId,
    int RevokedByUserId
) : IRequest<bool>;

public record RestoreBlockCommand(
    Guid BlockId,
    int RestoredByUserId
) : IRequest<bool>;