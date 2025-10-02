using PurchaseService.Application;
using PurchaseService.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers();

// Add Application services
builder.Services.AddApplication();

// Add Infrastructure services  
builder.Services.AddInfrastructure(builder.Configuration);

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Purchase Service API", Version = "v1" });
});

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

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Apply database migrations
try
{
    await app.Services.MigrateDatabaseAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while migrating the database");
    return;
}

app.Run();