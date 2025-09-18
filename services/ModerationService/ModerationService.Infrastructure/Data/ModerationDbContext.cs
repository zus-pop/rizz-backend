using Microsoft.EntityFrameworkCore;
using ModerationService.Domain.Entities;
using ModerationService.Infrastructure.Configurations;

namespace ModerationService.Infrastructure.Data;

public class ModerationDbContext : DbContext
{
    public ModerationDbContext(DbContextOptions<ModerationDbContext> options) : base(options)
    {
    }

    public DbSet<Block> Blocks { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<ModerationCase> ModerationCases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BlockConfiguration());
        modelBuilder.ApplyConfiguration(new ReportConfiguration());
        modelBuilder.ApplyConfiguration(new ModerationCaseConfiguration());
    }
}