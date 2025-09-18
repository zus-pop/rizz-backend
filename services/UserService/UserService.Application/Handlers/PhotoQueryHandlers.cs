using AutoMapper;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Domain.Repositories;

namespace UserService.Application.Handlers
{
    public class PhotoQueryHandlers : 
        IRequestHandler<GetPhotoByIdQuery, PhotoDto?>,
        IRequestHandler<GetPhotosByUserIdQuery, IEnumerable<PhotoDto>>,
        IRequestHandler<GetMainPhotoByUserIdQuery, PhotoDto?>
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IMapper _mapper;

        public PhotoQueryHandlers(IPhotoRepository photoRepository, IMapper mapper)
        {
            _photoRepository = photoRepository;
            _mapper = mapper;
        }

        public async Task<PhotoDto?> Handle(GetPhotoByIdQuery request, CancellationToken cancellationToken)
        {
            var photo = await _photoRepository.GetByIdAsync(request.Id, cancellationToken);
            return photo != null ? _mapper.Map<PhotoDto>(photo) : null;
        }

        public async Task<IEnumerable<PhotoDto>> Handle(GetPhotosByUserIdQuery request, CancellationToken cancellationToken)
        {
            var photos = await _photoRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            return _mapper.Map<IEnumerable<PhotoDto>>(photos);
        }

        public async Task<PhotoDto?> Handle(GetMainPhotoByUserIdQuery request, CancellationToken cancellationToken)
        {
            var photo = await _photoRepository.GetMainPhotoByUserIdAsync(request.UserId, cancellationToken);
            return photo != null ? _mapper.Map<PhotoDto>(photo) : null;
        }
    }
}