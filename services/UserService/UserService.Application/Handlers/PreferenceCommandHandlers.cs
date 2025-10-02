using AutoMapper;
using MediatR;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using DomainPreference = UserService.Domain.Entities.Preference;

namespace UserService.Application.Handlers
{
    public class PreferenceCommandHandlers : 
        IRequestHandler<CreatePreferenceCommand, PreferenceDto>,
        IRequestHandler<UpdatePreferenceCommand, PreferenceDto>,
        IRequestHandler<DeletePreferenceCommand, bool>
    {
        private readonly IPreferenceRepository _preferenceRepository;
        private readonly IMapper _mapper;

        public PreferenceCommandHandlers(IPreferenceRepository preferenceRepository, IMapper mapper)
        {
            _preferenceRepository = preferenceRepository;
            _mapper = mapper;
        }

        public async Task<PreferenceDto> Handle(CreatePreferenceCommand request, CancellationToken cancellationToken)
        {
            var preference = new DomainPreference(request.UserId);
            
            preference.SetLookingForGender(request.LookingForGender);

            if (request.AgeMin.HasValue || request.AgeMax.HasValue)
                preference.SetAgeRange(request.AgeMin, request.AgeMax);
            
            if (request.MaxDistanceKm.HasValue)
                preference.SetLocationRadius((int)request.MaxDistanceKm.Value);
            
            if (!string.IsNullOrEmpty(request.InterestsFilter))
                preference.SetInterestsFilter(request.InterestsFilter);
            
            if (!string.IsNullOrEmpty(request.Emotion))
                preference.SetEmotion(request.Emotion);
                
            if (!string.IsNullOrEmpty(request.VoiceQuality))
                preference.SetVoiceQuality(request.VoiceQuality);
                
            if (!string.IsNullOrEmpty(request.Accent))
                preference.SetAccent(request.Accent);

            var createdPreference = await _preferenceRepository.AddAsync(preference, cancellationToken);
            return _mapper.Map<PreferenceDto>(createdPreference);
        }

        public async Task<PreferenceDto> Handle(UpdatePreferenceCommand request, CancellationToken cancellationToken)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (preference == null)
                throw new ArgumentException("Preference not found");

            if (request.LookingForGender.HasValue)
                preference.SetLookingForGender(request.LookingForGender.Value);
            
            if (request.AgeMin.HasValue || request.AgeMax.HasValue)
                preference.SetAgeRange(request.AgeMin, request.AgeMax);
            
            if (request.MaxDistanceKm.HasValue)
                preference.SetLocationRadius((int)request.MaxDistanceKm.Value);
            
            if (request.InterestsFilter != null)
                preference.SetInterestsFilter(request.InterestsFilter);
            
            if (!string.IsNullOrEmpty(request.Emotion))
                preference.SetEmotion(request.Emotion);
                
            if (!string.IsNullOrEmpty(request.VoiceQuality))
                preference.SetVoiceQuality(request.VoiceQuality);
                
            if (!string.IsNullOrEmpty(request.Accent))
                preference.SetAccent(request.Accent);

            await _preferenceRepository.UpdateAsync(preference, cancellationToken);
            return _mapper.Map<PreferenceDto>(preference);
        }

        public async Task<bool> Handle(DeletePreferenceCommand request, CancellationToken cancellationToken)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (preference == null)
                return false;

            await _preferenceRepository.DeleteAsync(preference.Id, cancellationToken);
            return true;
        }
    }
}