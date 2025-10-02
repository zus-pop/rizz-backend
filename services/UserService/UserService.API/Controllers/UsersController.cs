using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] bool? verified = null, [FromQuery] string? gender = null)
        {
            try
            {
                var query = new GetAllUsersQuery { Verified = verified, Gender = gender };
                var users = await _mediator.Send(query);

                return Ok(new
                {
                    success = true,
                    data = users.Select(user => new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.PhoneNumber,
                        user.Gender,
                        user.Age,
                        user.Height,
                        user.Personality,
                        user.IsVerified,
                        user.VerifiedAt,
                        user.CreatedAt,
                        location = user.Location != null ? new { user.Location.Latitude, user.Location.Longitude } : null
                    }),
                    count = users.Count()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var query = new GetUserByIdQuery { Id = id };
                var user = await _mediator.Send(query);

                if (user == null)
                    return NotFound(new { success = false, message = "User not found" });

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.FullName,
                        user.Email,
                        user.PhoneNumber,
                        user.Gender,
                        user.Birthday,
                        user.Age,
                        user.Height,
                        user.Personality,
                        user.IsVerified,
                        user.VerifiedAt,
                        user.CreatedAt,
                        user.UpdatedAt,
                        location = user.Location != null ? new { user.Location.Latitude, user.Location.Longitude } : null
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var query = new GetUserByEmailQuery { Email = email };
                var user = await _mediator.Send(query);

                if (user == null)
                    return NotFound(new { success = false, message = "User not found" });

                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("phone/{phoneNumber}")]
        public async Task<IActionResult> GetUserByPhoneNumber(string phoneNumber)
        {
            try
            {
                var query = new GetUserByPhoneNumberQuery { PhoneNumber = phoneNumber };
                var user = await _mediator.Send(query);

                if (user == null)
                    return NotFound(new { success = false, message = "User not found" });

                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyUsers([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radius = 50)
        {
            try
            {
                var query = new GetNearbyUsersQuery 
                { 
                    Latitude = latitude, 
                    Longitude = longitude, 
                    RadiusKm = radius 
                };
                var users = await _mediator.Send(query);

                return Ok(new
                {
                    success = true,
                    data = users,
                    count = users.Count()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                var command = new CreateUserCommand
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Gender = dto.Gender,
                    Personality = dto.Personality,
                    Birthday = dto.Birthday,
                    Height = dto.Height,
                    Location = dto.Location
                };

                var user = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
                {
                    success = true,
                    data = user,
                    message = "User created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var command = new UpdateUserCommand
                {
                    Id = id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Gender = dto.Gender,
                    Personality = dto.Personality,
                    Birthday = dto.Birthday,
                    Height = dto.Height,
                    Location = dto.Location
                };

                var user = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    data = user,
                    message = "User updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var command = new DeleteUserCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result)
                    return NotFound(new { success = false, message = "User not found" });

                return Ok(new
                {
                    success = true,
                    message = "User deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/verify")]
        public async Task<IActionResult> VerifyUser(int id)
        {
            try
            {
                var command = new VerifyUserCommand { Id = id };
                var user = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    data = user,
                    message = "User verified successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/unverify")]
        public async Task<IActionResult> UnverifyUser(int id)
        {
            try
            {
                var command = new UnverifyUserCommand { Id = id };
                var user = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    data = user,
                    message = "User unverified successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}