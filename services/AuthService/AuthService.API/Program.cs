using AuthService.Infrastructure.Data;
using AuthService.API;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthService(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Authentication Service API",
        Version = "v1",
        Description = "API for user authentication, registration, and OTP verification"
    });
});

var app = builder.Build();

// Database migration
try {
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await context.Database.MigrateAsync();
    // Temporarily disable seeding to test registration
    // await AuthSampleDataSeeder.SeedSampleDataAsync(context);
}
catch (Exception ex) {
    Console.WriteLine($"Migration failed: {ex.Message}");
}

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication Service API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRateLimiter();
app.MapControllers();
app.Urls.Add("http://0.0.0.0:8080");
app.Run();
