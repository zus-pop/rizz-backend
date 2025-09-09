using Microsoft.EntityFrameworkCore;
using AiInsightsService.API.Data;
using AiInsightsService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AiDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<AiRabbitMqListener>();

var app = builder.Build();
app.MapControllers();
app.Run();
