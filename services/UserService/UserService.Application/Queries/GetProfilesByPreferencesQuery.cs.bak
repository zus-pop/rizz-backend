using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Queries
{
    public class GetProfilesByPreferencesQuery : IRequest<IEnumerable<ProfileWithUserDto>>
    {
        public int UserId { get; set; }
        public double? UserLatitude { get; set; }
        public double? UserLongitude { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}