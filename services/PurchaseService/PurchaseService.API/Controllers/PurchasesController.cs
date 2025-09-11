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

        [HttpGet]
        public async Task<IActionResult> GetAllPurchases([FromQuery] string? status = null, [FromQuery] int? userId = null)
        {
            try
            {
                var query = _context.Purchases.AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(p => p.Status == status);
                }

                if (userId.HasValue)
                {
                    query = query.Where(p => p.UserId == userId.Value);
                }

                var purchases = await query
                    .OrderByDescending(p => p.PurchasedAt)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = purchases,
                    total = purchases.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchase(int id)
        {
            try
            {
                var purchase = await _context.Purchases.FindAsync(id);
                if (purchase == null)
                {
                    return NotFound(new { success = false, message = "Purchase not found" });
                }

                return Ok(new { success = true, data = purchase });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
                }

                var purchase = new Purchase
                {
                    UserId = request.UserId,
                    Currency = request.Currency ?? "USD",
                    PurchasedAt = DateTime.UtcNow,
                    Status = "pending"
                };

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                // Simulate payment processing
                if (request.AutoComplete != false)
                {
                    purchase.Status = "completed";
                    purchase.StartTime = DateTime.UtcNow;
                    purchase.EndTime = DateTime.UtcNow.AddMonths(request.DurationMonths ?? 1);
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(GetPurchase), new { id = purchase.Id }, 
                    new { success = true, data = purchase });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompletePurchase(int id, [FromBody] CompletePurchaseRequest? request = null)
        {
            try
            {
                var purchase = await _context.Purchases.FindAsync(id);
                if (purchase == null)
                {
                    return NotFound(new { success = false, message = "Purchase not found" });
                }

                if (purchase.Status != "pending")
                {
                    return BadRequest(new { success = false, message = "Purchase is not in pending status" });
                }

                purchase.Status = "completed";
                purchase.StartTime = DateTime.UtcNow;
                purchase.EndTime = DateTime.UtcNow.AddMonths(request?.DurationMonths ?? 1);
                
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = purchase, message = "Purchase completed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelPurchase(int id, [FromBody] CancelPurchaseRequest? request = null)
        {
            try
            {
                var purchase = await _context.Purchases.FindAsync(id);
                if (purchase == null)
                {
                    return NotFound(new { success = false, message = "Purchase not found" });
                }

                if (purchase.Status == "completed")
                {
                    return BadRequest(new { success = false, message = "Cannot cancel completed purchase" });
                }

                purchase.Status = "failed";
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = purchase, message = "Purchase cancelled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPurchases(int userId, [FromQuery] string? status = null)
        {
            try
            {
                var query = _context.Purchases.Where(p => p.UserId == userId);

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(p => p.Status == status);
                }

                var purchases = await query
                    .OrderByDescending(p => p.PurchasedAt)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = purchases,
                    total = purchases.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/active")]
        public async Task<IActionResult> GetActiveUserPurchases(int userId)
        {
            try
            {
                var now = DateTime.UtcNow;
                var activePurchases = await _context.Purchases
                    .Where(p => p.UserId == userId && 
                               p.Status == "completed" && 
                               p.EndTime.HasValue && 
                               p.EndTime.Value > now)
                    .OrderBy(p => p.EndTime)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = activePurchases,
                    total = activePurchases.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/stats")]
        public async Task<IActionResult> GetUserPurchaseStats(int userId)
        {
            try
            {
                var totalPurchases = await _context.Purchases.CountAsync(p => p.UserId == userId);
                var completedPurchases = await _context.Purchases.CountAsync(p => p.UserId == userId && p.Status == "completed");
                var pendingPurchases = await _context.Purchases.CountAsync(p => p.UserId == userId && p.Status == "pending");
                var failedPurchases = await _context.Purchases.CountAsync(p => p.UserId == userId && p.Status == "failed");

                var now = DateTime.UtcNow;
                var activePurchases = await _context.Purchases.CountAsync(p => p.UserId == userId && 
                                                                              p.Status == "completed" && 
                                                                              p.EndTime.HasValue && 
                                                                              p.EndTime.Value > now);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        total = totalPurchases,
                        completed = completedPurchases,
                        pending = pendingPurchases,
                        failed = failedPurchases,
                        active = activePurchases
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            try
            {
                var purchase = await _context.Purchases.FindAsync(id);
                if (purchase == null)
                {
                    return NotFound(new { success = false, message = "Purchase not found" });
                }

                _context.Purchases.Remove(purchase);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Purchase deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetPurchaseStats()
        {
            try
            {
                var totalPurchases = await _context.Purchases.CountAsync();
                var completedPurchases = await _context.Purchases.CountAsync(p => p.Status == "completed");
                var pendingPurchases = await _context.Purchases.CountAsync(p => p.Status == "pending");
                var failedPurchases = await _context.Purchases.CountAsync(p => p.Status == "failed");

                var now = DateTime.UtcNow;
                var activePurchases = await _context.Purchases.CountAsync(p => p.Status == "completed" && 
                                                                              p.EndTime.HasValue && 
                                                                              p.EndTime.Value > now);

                var currencyStats = await _context.Purchases
                    .GroupBy(p => p.Currency)
                    .Select(g => new { Currency = g.Key, Count = g.Count() })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        total = totalPurchases,
                        completed = completedPurchases,
                        pending = pendingPurchases,
                        failed = failedPurchases,
                        active = activePurchases,
                        currencyBreakdown = currencyStats
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class CreatePurchaseRequest
    {
        public int UserId { get; set; }
        public string Currency { get; set; } = "USD";
        public int? DurationMonths { get; set; } = 1;
        public bool? AutoComplete { get; set; } = true;
    }

    public class CompletePurchaseRequest
    {
        public int? DurationMonths { get; set; } = 1;
    }

    public class CancelPurchaseRequest
    {
        public string? Reason { get; set; }
    }
}
