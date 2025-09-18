using MediatR;
using ModerationService.Application.DTOs;

namespace ModerationService.Application.Queries;

public record GetBlockByIdQuery(Guid BlockId) : IRequest<BlockDto?>;

public record GetUserBlocksQuery(int UserId) : IRequest<List<BlockDto>>;

public record GetBlocksForUserQuery(int BlockedUserId) : IRequest<List<BlockDto>>;

public record CheckActiveBlockQuery(int BlockerId, int BlockedUserId) : IRequest<bool>;