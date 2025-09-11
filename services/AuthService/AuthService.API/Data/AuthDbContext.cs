using Microsoft.EntityFrameworkCore;
using AuthService.API.Models;
using Common.Infrastructure.Data;

namespace AuthService.API.Data
{
    public class AuthDbContext : BaseDbContext
    {
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    }
}
