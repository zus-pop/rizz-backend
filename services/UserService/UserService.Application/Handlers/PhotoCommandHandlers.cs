using AutoMapper;
using MediatR;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.Handlers
{
    public class PhotoCommandHandlers : 
        IRequestHandler<CreatePhotoCommand, PhotoDto>,
        IRequestHandler<UpdatePhotoCommand, PhotoDto>,
        IRequestHandler<DeletePhotoCommand, bool>,
        IRequestHandler<SetMainPhotoCommand, PhotoDto>
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PhotoCommandHandlers(IPhotoRepository photoRepository, IUserRepository userRepository, IMapper mapper)
        {
            _photoRepository = photoRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PhotoDto> Handle(CreatePhotoCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            if (!await _userRepository.ExistsAsync(request.UserId, cancellationToken))
                throw new ArgumentException("User not found");

            var photo = new Photo(request.UserId, request.Url, request.DisplayOrder);
            
            if (!string.IsNullOrEmpty(request.Description))
                photo.SetDescription(request.Description);

            var createdPhoto = await _photoRepository.AddAsync(photo, cancellationToken);
            return _mapper.Map<PhotoDto>(createdPhoto);
        }

        public async Task<PhotoDto> Handle(UpdatePhotoCommand request, CancellationToken cancellationToken)
        {
            var photo = await _photoRepository.GetByIdAsync(request.Id, cancellationToken);
            if (photo == null)
                throw new ArgumentException("Photo not found");

            if (request.Description != null)
                photo.SetDescription(request.Description);
            
            if (request.DisplayOrder.HasValue)
                photo.SetDisplayOrder(request.DisplayOrder.Value);
            
            if (request.IsMainPhoto.HasValue)
            {
                if (request.IsMainPhoto.Value)
                    photo.SetAsMainPhoto();
                else
                    photo.RemoveAsMainPhoto();
            }

            await _photoRepository.UpdateAsync(photo, cancellationToken);
            return _mapper.Map<PhotoDto>(photo);
        }

        public async Task<bool> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            var photo = await _photoRepository.GetByIdAsync(request.Id, cancellationToken);
            if (photo == null)
                return false;

            await _photoRepository.DeleteAsync(request.Id, cancellationToken);
            return true;
        }

        public async Task<PhotoDto> Handle(SetMainPhotoCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            if (!await _userRepository.ExistsAsync(request.UserId, cancellationToken))
                throw new ArgumentException("User not found");

            // Check if photo exists and belongs to the user
            var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);
            if (photo == null || photo.UserId != request.UserId)
                throw new ArgumentException("Photo not found or does not belong to the user");

            await _photoRepository.SetMainPhotoAsync(request.UserId, request.PhotoId, cancellationToken);
            
            // Reload the updated photo
            var updatedPhoto = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);
            return _mapper.Map<PhotoDto>(updatedPhoto!);
        }
    }
}