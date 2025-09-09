using Microsoft.EntityFrameworkCore;
using NotificationService.API.Data;
using NotificationService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<NotificationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<RabbitMqListener>();

var app = builder.Build();

app.MapControllers();
app.Run();
