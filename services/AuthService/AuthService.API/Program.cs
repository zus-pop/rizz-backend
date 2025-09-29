using AuthService.Infrastructure.Data;
using AuthService.API;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthService(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Database migration
try {
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await context.Database.MigrateAsync();
    await AuthSampleDataSeeder.SeedSampleDataAsync(context);
}
catch (Exception ex) {
    Console.WriteLine($"Migration failed: {ex.Message}");
}

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.MapControllers();
app.Urls.Add("http://0.0.0.0:8081");
app.Run();
