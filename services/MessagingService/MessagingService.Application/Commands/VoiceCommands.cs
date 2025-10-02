using MediatR;
using Microsoft.AspNetCore.Http;
using MessagingService.Application.DTOs;

namespace MessagingService.Application.Commands
{
    public class SendVoiceMessageCommand : IRequest<MessageDto>
    {
        public int MatchId { get; set; }
        public int SenderId { get; set; }
        public IFormFile VoiceFile { get; set; } = null!;
    }
}