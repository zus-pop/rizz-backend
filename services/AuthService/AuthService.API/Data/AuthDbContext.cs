using Microsoft.EntityFrameworkCore;
using AuthService.API.Models;

namespace AuthService.API.Data
{
    public class AuthDbContext : DbContext
    {
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    }
}
