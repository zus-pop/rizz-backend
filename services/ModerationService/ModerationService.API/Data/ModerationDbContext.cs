using Microsoft.EntityFrameworkCore;
using ModerationService.API.Models;

namespace ModerationService.API.Data
{
    public class ModerationDbContext : DbContext
    {
        public ModerationDbContext(DbContextOptions<ModerationDbContext> options) : base(options) { }

        public DbSet<Block> Blocks { get; set; }
        public DbSet<Report> Reports { get; set; }
    }
}
