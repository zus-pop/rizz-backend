using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Queries
{
    public class GetPreferenceByUserIdQuery : IRequest<PreferenceDto?>
    {
        public int UserId { get; set; }
    }
}