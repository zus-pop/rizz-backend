using Microsoft.EntityFrameworkCore;
using MessagingService.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<MessagingDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.MapControllers();
app.Run();
