using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Queries
{
    public class GetProfileByUserIdQuery : IRequest<ProfileDto?>
    {
        public int UserId { get; set; }
    }

    public class GetProfileByIdQuery : IRequest<ProfileDto?>
    {
        public int Id { get; set; }
    }
}