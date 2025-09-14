using Microsoft.EntityFrameworkCore;
using PurchaseService.API.Models;

namespace PurchaseService.API.Data
{
    public class PurchaseDbContext : DbContext
    {
        public PurchaseDbContext(DbContextOptions<PurchaseDbContext> options) : base(options) { }

        public DbSet<Purchase> Purchases { get; set; }
    }
}
