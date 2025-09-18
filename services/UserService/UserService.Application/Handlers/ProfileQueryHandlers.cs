using AutoMapper;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Domain.Repositories;

namespace UserService.Application.Handlers
{
    public class ProfileQueryHandlers : 
        IRequestHandler<GetProfileByIdQuery, ProfileDto?>,
        IRequestHandler<GetProfileByUserIdQuery, ProfileDto?>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public ProfileQueryHandlers(IProfileRepository profileRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public async Task<ProfileDto?> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByIdAsync(request.Id, cancellationToken);
            return profile != null ? _mapper.Map<ProfileDto>(profile) : null;
        }

        public async Task<ProfileDto?> Handle(GetProfileByUserIdQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            return profile != null ? _mapper.Map<ProfileDto>(profile) : null;
        }
    }
}