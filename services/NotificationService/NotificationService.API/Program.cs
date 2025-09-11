using Microsoft.EntityFrameworkCore;
using NotificationService.API.Data;
using NotificationService.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

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

// Add Entity Framework
builder.Services.AddDbContext<NotificationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add RabbitMQ listener service only in production or when explicitly enabled
if (builder.Environment.IsProduction() || 
    builder.Configuration.GetValue<bool>("EnableRabbitMQ", false))
{
    builder.Services.AddHostedService<RabbitMqListener>();
    Console.WriteLine("RabbitMQ listener enabled");
}
else
{
    Console.WriteLine("RabbitMQ listener disabled for development");
}

// Add Swagger for development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowAll");

// Map controllers
app.MapControllers();

Console.WriteLine("NotificationService.API is starting...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("Available endpoints:");
Console.WriteLine("- GET / (Service information)");
Console.WriteLine("- GET /health (Health check)");
Console.WriteLine("- GET /api/notification/user/{userId} (Get user notifications)");
Console.WriteLine("- POST /api/notification (Create notification)");
Console.WriteLine("- PUT /api/notification/{id}/mark-read (Mark as read)");
Console.WriteLine("- And more...");

app.Run();
