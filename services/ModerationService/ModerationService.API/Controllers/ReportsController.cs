using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModerationService.API.Data;
using ModerationService.API.Models;

namespace ModerationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ModerationDbContext _context;

        public ReportsController(ModerationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ReportUser([FromBody] Report report)
        {
            report.CreatedAt = DateTime.UtcNow;
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return Ok(report);
        }

        [HttpGet("reported/{userId}")]
        public async Task<IActionResult> GetReportsAgainstUser(int userId)
        {
            var reports = await _context.Reports
                .Where(r => r.ReportedId == userId)
                .ToListAsync();
            return Ok(reports);
        }
    }
}
