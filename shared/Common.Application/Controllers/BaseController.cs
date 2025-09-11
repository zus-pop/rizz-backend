using Microsoft.AspNetCore.Mvc;
using Common.Application.Models;

namespace Common.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResult<T>(ApiResponse<T> response)
        {
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        protected IActionResult HandleException(Exception ex)
        {
            var response = ApiResponse<object>.Failure(ex.Message);
            return StatusCode(500, response);
        }
    }
}
