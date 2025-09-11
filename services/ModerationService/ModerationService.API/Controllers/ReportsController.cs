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

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "ModerationService - Reports", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _context.Reports
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return Ok(reports);
        }

        [HttpPost]
        public async Task<IActionResult> ReportUser([FromBody] Report report)
        {
            try
            {
                if (report == null || report.ReporterId <= 0 || report.ReportedId <= 0)
                    return BadRequest("Valid ReporterId and ReportedId are required");

                if (report.ReporterId == report.ReportedId)
                    return BadRequest("User cannot report themselves");

                if (string.IsNullOrWhiteSpace(report.Reason))
                    return BadRequest("Report reason is required");

                report.CreatedAt = DateTime.UtcNow;
                _context.Reports.Add(report);
                await _context.SaveChangesAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to report user", details = ex.Message });
            }
        }

        [HttpGet("reported/{userId}")]
        public async Task<IActionResult> GetReportsAgainstUser(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest("Valid UserId is required");

                var reports = await _context.Reports
                    .Where(r => r.ReportedId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                var reportCount = reports.Count;
                var reasonGroups = reports
                    .GroupBy(r => r.Reason)
                    .Select(g => new { reason = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .ToList();

                return Ok(new 
                { 
                    userId, 
                    totalReports = reportCount, 
                    reports, 
                    reasonBreakdown = reasonGroups 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to get reports against user", details = ex.Message });
            }
        }

        [HttpGet("reporter/{userId}")]
        public async Task<IActionResult> GetReportsByUser(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest("Valid UserId is required");

                var reports = await _context.Reports
                    .Where(r => r.ReporterId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to get reports by user", details = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetReportStats()
        {
            try
            {
                var totalReports = await _context.Reports.CountAsync();
                var todayReports = await _context.Reports
                    .Where(r => r.CreatedAt.Date == DateTime.UtcNow.Date)
                    .CountAsync();

                var topReasons = await _context.Reports
                    .GroupBy(r => r.Reason)
                    .Select(g => new { reason = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .Take(5)
                    .ToListAsync();

                var mostReportedUsers = await _context.Reports
                    .GroupBy(r => r.ReportedId)
                    .Select(g => new { userId = g.Key, reportCount = g.Count() })
                    .OrderByDescending(x => x.reportCount)
                    .Take(10)
                    .ToListAsync();

                return Ok(new 
                { 
                    totalReports, 
                    todayReports, 
                    topReasons, 
                    mostReportedUsers,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to get report statistics", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Valid report ID is required");

                var report = await _context.Reports.FindAsync(id);
                if (report == null)
                    return NotFound($"Report with id {id} not found");

                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Report {id} deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to delete report", details = ex.Message });
            }
        }
    }
}
