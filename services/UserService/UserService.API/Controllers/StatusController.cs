using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("info")]
        public IActionResult GetSystemInfo()
        {
            return Ok(new
            {
                service = "UserService",
                version = "2.0.0",
                architecture = "Clean Architecture + DDD + CQRS",
                status = "running",
                timestamp = DateTime.UtcNow,
                features = new[]
                {
                    "Domain-Driven Design",
                    "Command Query Responsibility Segregation (CQRS)",
                    "MediatR Pattern",
                    "Entity Framework Core with PostgreSQL",
                    "AutoMapper",
                    "FluentValidation",
                    "Clean Architecture Layers"
                }
            });
        }

        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                checks = new
                {
                    api = "healthy",
                    mediator = _mediator != null ? "healthy" : "unhealthy"
                }
            });
        }
    }
}