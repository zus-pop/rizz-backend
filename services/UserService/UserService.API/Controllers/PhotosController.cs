using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PhotosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            try
            {
                var query = new GetPhotoByIdQuery { Id = id };
                var photo = await _mediator.Send(query);

                if (photo == null)
                    return NotFound(new { success = false, message = "Photo not found" });

                return Ok(new { success = true, data = photo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPhotosByUserId(int userId)
        {
            try
            {
                var query = new GetPhotosByUserIdQuery { UserId = userId };
                var photos = await _mediator.Send(query);

                return Ok(new
                {
                    success = true,
                    data = photos,
                    count = photos.Count()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/main")]
        public async Task<IActionResult> GetMainPhotoByUserId(int userId)
        {
            try
            {
                var query = new GetMainPhotoByUserIdQuery { UserId = userId };
                var photo = await _mediator.Send(query);

                if (photo == null)
                    return NotFound(new { success = false, message = "Main photo not found" });

                return Ok(new { success = true, data = photo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePhoto([FromBody] CreatePhotoRequest request)
        {
            try
            {
                var command = new CreatePhotoCommand
                {
                    UserId = request.UserId,
                    Url = request.Url,
                    Description = request.Description,
                    DisplayOrder = request.DisplayOrder
                };

                var photo = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetPhoto), new { id = photo.Id }, new
                {
                    success = true,
                    data = photo,
                    message = "Photo created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhoto(int id, [FromBody] UpdatePhotoDto dto)
        {
            try
            {
                var command = new UpdatePhotoCommand
                {
                    Id = id,
                    Description = dto.Description,
                    DisplayOrder = dto.DisplayOrder,
                    IsMainPhoto = dto.IsMainPhoto
                };

                var photo = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    data = photo,
                    message = "Photo updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("user/{userId}/main/{photoId}")]
        public async Task<IActionResult> SetMainPhoto(int userId, int photoId)
        {
            try
            {
                var command = new SetMainPhotoCommand { UserId = userId, PhotoId = photoId };
                var photo = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    data = photo,
                    message = "Main photo set successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            try
            {
                var command = new DeletePhotoCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result)
                    return NotFound(new { success = false, message = "Photo not found" });

                return Ok(new
                {
                    success = true,
                    message = "Photo deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CreatePhotoRequest
    {
        public int UserId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}