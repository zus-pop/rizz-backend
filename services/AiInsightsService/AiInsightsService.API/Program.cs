using Microsoft.EntityFrameworkCore;
using AiInsightsService.API.Data;
using AiInsightsService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AiDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Only add RabbitMQ listener if RabbitMQ is available (for development, we can skip this)
// builder.Services.AddHostedService<AiRabbitMqListener>();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

app.MapControllers();
app.Run();
