using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Domain.Repositories;

namespace UserService.Application.Handlers
{
    public class GetProfilesByPreferencesQueryHandler : IRequestHandler<GetProfilesByPreferencesQuery, IEnumerable<ProfileWithUserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;

        public GetProfilesByPreferencesQueryHandler(IUserRepository userRepository, IProfileRepository profileRepository)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
        }

        public async Task<IEnumerable<ProfileWithUserDto>> Handle(GetProfilesByPreferencesQuery request, CancellationToken cancellationToken)
        {
            // Get the current user's profile and their preferences
            var currentUserProfile = await _profileRepository.GetByUserIdAsync(request.UserId);
            if (currentUserProfile == null)
                return new List<ProfileWithUserDto>();

            // Get potential matches based on preferences
            var profiles = await _profileRepository.GetProfilesByPreferencesAsync(
                currentUserId: request.UserId,
                interestedInGender: currentUserProfile.InterestedInGender,
                minAge: currentUserProfile.InterestedInAgeMin,
                maxAge: currentUserProfile.InterestedInAgeMax,
                userLatitude: request.UserLatitude,
                userLongitude: request.UserLongitude,
                maxDistanceKm: currentUserProfile.MaxDistanceKm,
                showOnlyVerified: currentUserProfile.ShowOnlyVerified,
                page: request.Page,
                pageSize: request.PageSize
            );

            // Get users for the profiles to include full user data
            var userIds = profiles.Select(p => p.UserId).ToList();
            var users = new List<UserService.Domain.Entities.User>();
            
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                    users.Add(user);
            }

            return profiles.Select(p =>
            {
                var user = users.First(u => u.Id == p.UserId);
                
                // Calculate distance if user location is provided
                double? distanceKm = null;
                if (request.UserLatitude.HasValue && request.UserLongitude.HasValue && user.Location != null)
                {
                    var userLocation = UserService.Domain.ValueObjects.Location.Create(request.UserLatitude.Value, request.UserLongitude.Value);
                    var distanceMeters = user.Location.Point.Distance(userLocation.Point);
                    distanceKm = distanceMeters / 1000.0; // Convert to kilometers
                }

                return new ProfileWithUserDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Bio = p.Bio,
                    Job = p.Job,
                    School = p.School,
                    InterestedInAgeMin = p.InterestedInAgeMin,
                    InterestedInAgeMax = p.InterestedInAgeMax,
                    InterestedInGender = p.InterestedInGender,
                    MaxDistanceKm = p.MaxDistanceKm,
                    ShowOnlyVerified = p.ShowOnlyVerified,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    DistanceKm = distanceKm,
                    User = new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FullName = user.FullName,
                        Email = user.Email.ToString(),
                        PhoneNumber = user.PhoneNumber.ToString(),
                        Gender = user.Gender,
                        Birthday = user.Birthday,
                        Age = user.Age,
                        Height = user.Height,
                        Personality = user.Personality,
                        IsVerified = user.IsVerified,
                        VerifiedAt = user.VerifiedAt,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt,
                        Location = user.Location != null ? new LocationDto
                        {
                            Latitude = user.Location.Latitude,
                            Longitude = user.Location.Longitude
                        } : null,
                        Photos = user.Photos?.Select(photo => new PhotoDto
                        {
                            Id = photo.Id,
                            UserId = photo.UserId,
                            Url = photo.Url,
                            Description = photo.Description,
                            IsMain = photo.IsMainPhoto,
                            Order = photo.DisplayOrder,
                            CreatedAt = photo.CreatedAt,
                            UpdatedAt = photo.UpdatedAt
                        }).ToList() ?? new List<PhotoDto>()
                    }
                };
            });
        }
    }
}