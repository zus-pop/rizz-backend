using MediatR;
using MessagingService.Application.DTOs;

namespace MessagingService.Application.Queries
{
    public class GetMessagesQuery : IRequest<IEnumerable<MessageDto>>
    {
        public int MatchId { get; set; }
        public int? UserId { get; set; } // For authorization
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class GetMessageByIdQuery : IRequest<MessageDto?>
    {
        public int MessageId { get; set; }
        public int? UserId { get; set; } // For authorization
    }

    public class GetUnreadMessagesQuery : IRequest<IEnumerable<MessageDto>>
    {
        public int UserId { get; set; }
    }

    public class GetConversationQuery : IRequest<IEnumerable<MessageDto>>
    {
        public int MatchId { get; set; }
        public int UserId { get; set; }
        public DateTime? Since { get; set; }
        public int Limit { get; set; } = 50;
    }
}