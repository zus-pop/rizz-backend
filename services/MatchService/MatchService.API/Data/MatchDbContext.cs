using Microsoft.EntityFrameworkCore;
using MatchService.API.Models;

namespace MatchService.API.Data
{
    public class MatchDbContext : DbContext
    {
        public DbSet<Match> Matches { get; set; }
        public DbSet<Swipe> Swipes { get; set; }

        public MatchDbContext(DbContextOptions<MatchDbContext> options) : base(options) { }
    }
}
