using Microsoft.EntityFrameworkCore;
using PurchaseService.API.Data;

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
builder.Services.AddDbContext<PurchaseDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

Console.WriteLine("PurchaseService.API is starting...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("Available endpoints:");
Console.WriteLine("- GET / (Service information)");
Console.WriteLine("- GET /health (Health check)");
Console.WriteLine("- GET /api/purchases (Get all purchases)");
Console.WriteLine("- POST /api/purchases (Create purchase)");
Console.WriteLine("- GET /api/purchases/user/{userId} (Get user purchases)");
Console.WriteLine("- GET /api/purchases/stats (Purchase statistics)");
Console.WriteLine("- And more...");

app.Run();
