using Microsoft.EntityFrameworkCore;
using MatchService.API.Data;
using MatchService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MatchDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Only add RabbitMQ if available (for development, we can skip this)
// builder.Services.AddSingleton<RabbitMqService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
