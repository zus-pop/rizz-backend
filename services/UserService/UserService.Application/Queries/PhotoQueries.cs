using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Queries
{
    public class GetPhotosByUserIdQuery : IRequest<IEnumerable<PhotoDto>>
    {
        public int UserId { get; set; }
    }

    public class GetPhotoByIdQuery : IRequest<PhotoDto?>
    {
        public int Id { get; set; }
    }

    public class GetMainPhotoByUserIdQuery : IRequest<PhotoDto?>
    {
        public int UserId { get; set; }
    }
}