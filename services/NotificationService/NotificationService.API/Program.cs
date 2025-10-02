using NotificationService.Application;
using NotificationService.Infrastructure;
using NotificationService.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<NotificationService.Infrastructure.Data.NotificationDbContext>();

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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Apply migrations on startup
await app.ApplyMigrationsAsync();

try
{
    Log.Information("Starting NotificationService API");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "NotificationService API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}