using AutoMapper;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Domain.Repositories;

namespace UserService.Application.Handlers
{
    public class UserQueryHandlers : 
        IRequestHandler<GetUserByIdQuery, UserDto?>,
        IRequestHandler<GetUserByEmailQuery, UserDto?>,
        IRequestHandler<GetUserByPhoneNumberQuery, UserDto?>,
        IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>,
        IRequestHandler<GetNearbyUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserQueryHandlers(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> Handle(GetUserByPhoneNumberQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetFilteredAsync(request.Verified, request.Gender, cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<IEnumerable<UserDto>> Handle(GetNearbyUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetNearbyUsersAsync(
                request.Latitude, 
                request.Longitude, 
                request.RadiusKm, 
                cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
    }
}