using AutoMapper;
using MediatR;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using DomainProfile = UserService.Domain.Entities.Profile;

namespace UserService.Application.Handlers
{
    public class ProfileCommandHandlers : 
        IRequestHandler<CreateProfileCommand, ProfileDto>,
        IRequestHandler<UpdateProfileCommand, ProfileDto>,
        IRequestHandler<DeleteProfileCommand, bool>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ProfileCommandHandlers(IProfileRepository profileRepository, IUserRepository userRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ProfileDto> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            if (!await _userRepository.ExistsAsync(request.UserId, cancellationToken))
                throw new ArgumentException("User not found");

            // Check if profile already exists for this user
            if (await _profileRepository.ExistsAsync(request.UserId, cancellationToken))
                throw new ArgumentException("Profile already exists for this user");

            var profile = new DomainProfile(request.UserId);
            
            // New fields
            if (!string.IsNullOrEmpty(request.Bio))
                profile.SetBio(request.Bio);
            
            if (!string.IsNullOrEmpty(request.Voice))
                profile.SetVoice(request.Voice);
                
            if (!string.IsNullOrEmpty(request.University))
                profile.SetUniversity(request.University);
                
            if (!string.IsNullOrEmpty(request.InterestedIn))
                profile.SetInterestedIn(request.InterestedIn);
                
            if (!string.IsNullOrEmpty(request.LookingFor))
                profile.SetLookingFor(request.LookingFor);
                
            if (!string.IsNullOrEmpty(request.StudyStyle))
                profile.SetStudyStyle(request.StudyStyle);
                
            if (!string.IsNullOrEmpty(request.WeekendHobby))
                profile.SetWeekendHobby(request.WeekendHobby);
                
            if (!string.IsNullOrEmpty(request.CampusLife))
                profile.SetCampusLife(request.CampusLife);
                
            if (!string.IsNullOrEmpty(request.FuturePlan))
                profile.SetFuturePlan(request.FuturePlan);
                
            if (!string.IsNullOrEmpty(request.CommunicationPreference))
                profile.SetCommunicationPreference(request.CommunicationPreference);
                
            if (!string.IsNullOrEmpty(request.DealBreakers))
                profile.SetDealBreakers(request.DealBreakers);
                
            if (!string.IsNullOrEmpty(request.Zodiac))
                profile.SetZodiac(request.Zodiac);
                
            if (!string.IsNullOrEmpty(request.LoveLanguage))
                profile.SetLoveLanguage(request.LoveLanguage);
            
            // New Vietnamese localization fields
            if (!string.IsNullOrEmpty(request.Emotion))
                profile.SetEmotion(request.Emotion);
                
            if (!string.IsNullOrEmpty(request.VoiceQuality))
                profile.SetVoiceQuality(request.VoiceQuality);
                
            if (!string.IsNullOrEmpty(request.Accent))
                profile.SetAccent(request.Accent);
            
            // Legacy fields for backward compatibility - these will be ignored since methods don't exist
            // The legacy properties are kept in commands for backward compatibility but not processed

            var createdProfile = await _profileRepository.AddAsync(profile, cancellationToken);
            return _mapper.Map<ProfileDto>(createdProfile);
        }

        public async Task<ProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (profile == null)
                throw new ArgumentException("Profile not found");

            // New fields
            if (request.Bio != null)
                profile.SetBio(request.Bio);
                
            if (request.Voice != null)
                profile.SetVoice(request.Voice);
                
            if (request.University != null)
                profile.SetUniversity(request.University);
                
            if (request.InterestedIn != null)
                profile.SetInterestedIn(request.InterestedIn);
                
            if (request.LookingFor != null)
                profile.SetLookingFor(request.LookingFor);
                
            if (request.StudyStyle != null)
                profile.SetStudyStyle(request.StudyStyle);
                
            if (request.WeekendHobby != null)
                profile.SetWeekendHobby(request.WeekendHobby);
                
            if (request.CampusLife != null)
                profile.SetCampusLife(request.CampusLife);
                
            if (request.FuturePlan != null)
                profile.SetFuturePlan(request.FuturePlan);
                
            if (request.CommunicationPreference != null)
                profile.SetCommunicationPreference(request.CommunicationPreference);
                
            if (request.DealBreakers != null)
                profile.SetDealBreakers(request.DealBreakers);
                
            if (request.Zodiac != null)
                profile.SetZodiac(request.Zodiac);
                
            if (request.LoveLanguage != null)
                profile.SetLoveLanguage(request.LoveLanguage);
            
            // New Vietnamese localization fields
            if (!string.IsNullOrEmpty(request.Emotion))
                profile.SetEmotion(request.Emotion);
                
            if (!string.IsNullOrEmpty(request.VoiceQuality))
                profile.SetVoiceQuality(request.VoiceQuality);
                
            if (!string.IsNullOrEmpty(request.Accent))
                profile.SetAccent(request.Accent);
            
            // Legacy fields for backward compatibility - these will be ignored since methods don't exist
            // The legacy properties are kept in commands for backward compatibility but not processed

            await _profileRepository.UpdateAsync(profile, cancellationToken);
            return _mapper.Map<ProfileDto>(profile);
        }

        public async Task<bool> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (profile == null)
                return false;

            await _profileRepository.DeleteAsync(profile.Id, cancellationToken);
            return true;
        }
    }
}