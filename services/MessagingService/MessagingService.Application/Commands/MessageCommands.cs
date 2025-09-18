using MediatR;
using MessagingService.Application.DTOs;

namespace MessagingService.Application.Commands
{
    public class SendMessageCommand : IRequest<MessageDto>
    {
        public int MatchId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "text";
        public string? ExtraData { get; set; }
    }

    public class MarkMessageAsReadCommand : IRequest<MessageDto>
    {
        public int MessageId { get; set; }
        public int ReaderId { get; set; }
    }

    public class UpdateMessageCommand : IRequest<MessageDto>
    {
        public int MessageId { get; set; }
        public string? Content { get; set; }
        public string? ExtraData { get; set; }
    }

    public class DeleteMessageCommand : IRequest<bool>
    {
        public int MessageId { get; set; }
        public int UserId { get; set; } // For authorization
    }
}