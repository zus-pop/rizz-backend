using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Queries
{
    public class GetUserVoiceQuery : IRequest<VoiceFileDto?>
    {
        public int UserId { get; set; }
    }
}