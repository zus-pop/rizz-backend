using MediatR;
using Microsoft.AspNetCore.Http;
using UserService.Application.DTOs;

namespace UserService.Application.Commands
{
    public class UploadVoiceCommand : IRequest<VoiceUploadResultDto>
    {
        public int UserId { get; set; }
        public IFormFile VoiceFile { get; set; } = null!;
    }

    public class DeleteVoiceCommand : IRequest
    {
        public int UserId { get; set; }
    }
}