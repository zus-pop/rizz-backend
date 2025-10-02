using MediatR;
using MessagingService.Application.DTOs;

namespace MessagingService.Application.Queries
{
    public class GetVoiceMessageFileQuery : IRequest<VoiceFileResultDto?>
    {
        public int MessageId { get; set; }
        public int RequesterId { get; set; }
    }

    public class GetVoiceMessagesQuery : IRequest<IEnumerable<MessageDto>>
    {
        public int MatchId { get; set; }
        public int RequesterId { get; set; }
    }
}