using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Commands
{
    public class CreatePhotoCommand : IRequest<PhotoDto>
    {
        public int UserId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdatePhotoCommand : IRequest<PhotoDto>
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsMainPhoto { get; set; }
    }

    public class DeletePhotoCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class SetMainPhotoCommand : IRequest<PhotoDto>
    {
        public int UserId { get; set; }
        public int PhotoId { get; set; }
    }
}