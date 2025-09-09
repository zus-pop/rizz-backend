using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseService.API.Data;
using PurchaseService.API.Models;

namespace PurchaseService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly PurchaseDbContext _context;

        public PurchasesController(PurchaseDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchase([FromBody] Purchase request)
        {
            request.PurchasedAt = DateTime.UtcNow;
            request.Status = "pending";
            _context.Purchases.Add(request);
            await _context.SaveChangesAsync();

            // Simulate external payment callback → mark as completed
            request.Status = "completed";
            request.StartTime = DateTime.UtcNow;
            request.EndTime = DateTime.UtcNow.AddMonths(1);
            await _context.SaveChangesAsync();

            return Ok(request);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPurchases(int userId)
        {
            var purchases = await _context.Purchases
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PurchasedAt)
                .ToListAsync();
            return Ok(purchases);
        }
    }
}
