using AutoMapper;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Domain.Repositories;

namespace UserService.Application.Handlers
{
    public class PreferenceQueryHandlers : IRequestHandler<GetPreferenceByUserIdQuery, PreferenceDto?>
    {
        private readonly IPreferenceRepository _preferenceRepository;
        private readonly IMapper _mapper;
        public PreferenceQueryHandlers(IPreferenceRepository preferenceRepository, IMapper mapper)
        {
            _preferenceRepository = preferenceRepository;
            _mapper = mapper;
        }

        public async Task<PreferenceDto?> Handle(GetPreferenceByUserIdQuery request, CancellationToken cancellationToken)
        {
            var pref = await _preferenceRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            return pref == null ? null : _mapper.Map<PreferenceDto>(pref);
        }
    }
}