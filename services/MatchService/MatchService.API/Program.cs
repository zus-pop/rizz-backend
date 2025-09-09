using Microsoft.EntityFrameworkCore;
using MatchService.API.Data;
using MatchService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MatchDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<RabbitMqService>();

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();

app.Run();
