using AutoMapper;
using MediatR;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.Handlers
{
    public class UserCommandHandlers : 
        IRequestHandler<CreateUserCommand, UserDto>,
        IRequestHandler<UpdateUserCommand, UserDto>,
        IRequestHandler<DeleteUserCommand, bool>,
        IRequestHandler<VerifyUserCommand, UserDto>,
        IRequestHandler<UnverifyUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserCommandHandlers(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken: cancellationToken))
                throw new ArgumentException("Email already exists");

            // Check if phone number already exists
            if (await _userRepository.PhoneNumberExistsAsync(request.PhoneNumber, cancellationToken: cancellationToken))
                throw new ArgumentException("Phone number already exists");

            var user = new User(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.Gender,
                request.Personality
            );

            if (request.Birthday.HasValue)
                user.SetBirthday(request.Birthday.Value);

            if (request.Height.HasValue)
                user.SetHeight(request.Height.Value);

            if (request.Location != null)
                user.SetLocation(request.Location.Latitude, request.Location.Longitude);

            var createdUser = await _userRepository.AddAsync(user, cancellationToken);
            return _mapper.Map<UserDto>(createdUser);
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
                throw new ArgumentException("User not found");

            if (!string.IsNullOrEmpty(request.FirstName))
                user.SetFirstName(request.FirstName);

            if (!string.IsNullOrEmpty(request.LastName))
                user.SetLastName(request.LastName);

            if (!string.IsNullOrEmpty(request.Email))
            {
                if (await _userRepository.EmailExistsAsync(request.Email, request.Id, cancellationToken))
                    throw new ArgumentException("Email already exists");
                user.SetEmail(request.Email);
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                if (await _userRepository.PhoneNumberExistsAsync(request.PhoneNumber, request.Id, cancellationToken))
                    throw new ArgumentException("Phone number already exists");
                user.SetPhoneNumber(request.PhoneNumber);
            }

            if (!string.IsNullOrEmpty(request.Gender))
                user.SetGender(request.Gender);

            if (!string.IsNullOrEmpty(request.Personality))
                user.SetPersonality(request.Personality);

            if (request.Birthday.HasValue)
                user.SetBirthday(request.Birthday.Value);

            if (request.Height.HasValue)
                user.SetHeight(request.Height.Value);

            if (request.Location != null)
                user.SetLocation(request.Location.Latitude, request.Location.Longitude);

            await _userRepository.UpdateAsync(user, cancellationToken);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (!await _userRepository.ExistsAsync(request.Id, cancellationToken))
                return false;

            await _userRepository.DeleteAsync(request.Id, cancellationToken);
            return true;
        }

        public async Task<UserDto> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
                throw new ArgumentException("User not found");

            user.Verify();
            await _userRepository.UpdateAsync(user, cancellationToken);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> Handle(UnverifyUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
                throw new ArgumentException("User not found");

            user.Unverify();
            await _userRepository.UpdateAsync(user, cancellationToken);
            return _mapper.Map<UserDto>(user);
        }
    }
}