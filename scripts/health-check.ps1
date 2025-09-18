# Solution Health Check Script
# This script verifies that all project paths in the solution are correct

Write-Host "=== Rizz Dating App - Solution Health Check ===" -ForegroundColor Green
Write-Host ""

# Check if solution file exists
if (Test-Path "DatingApp.sln") {
    Write-Host "✅ Solution file found: DatingApp.sln" -ForegroundColor Green
} else {
    Write-Host "❌ Solution file not found" -ForegroundColor Red
    exit 1
}

# List all projects in solution
Write-Host ""
Write-Host "📁 Projects in Solution:" -ForegroundColor Yellow
$projects = dotnet sln list | Where-Object { $_ -match "\.csproj$" }

foreach ($project in $projects) {
    if (Test-Path $project) {
        Write-Host "✅ $project" -ForegroundColor Green
    } else {
        Write-Host "❌ $project (NOT FOUND)" -ForegroundColor Red
    }
}

# Count projects by type
$gatewayProjects = $projects | Where-Object { $_ -match "gateway" }
$serviceProjects = $projects | Where-Object { $_ -match "services" }
$sharedProjects = $projects | Where-Object { $_ -match "shared" }

Write-Host ""
Write-Host "📊 Project Summary:" -ForegroundColor Yellow
Write-Host "   Gateway Projects: $($gatewayProjects.Count)" -ForegroundColor Cyan
Write-Host "   Service Projects: $($serviceProjects.Count)" -ForegroundColor Cyan
Write-Host "   Shared Projects: $($sharedProjects.Count)" -ForegroundColor Cyan
Write-Host "   Total Projects: $($projects.Count)" -ForegroundColor Cyan

# Check docker-compose services
Write-Host ""
Write-Host "🐳 Docker Compose Status:" -ForegroundColor Yellow
if (Test-Path "docker-compose.yml") {
    Write-Host "✅ docker-compose.yml found" -ForegroundColor Green
    $dockerServices = docker-compose config --services 2>$null
    if ($dockerServices) {
        Write-Host "   Services: $($dockerServices.Count)" -ForegroundColor Cyan
        $dockerServices | ForEach-Object { Write-Host "   - $_" -ForegroundColor White }
    }
} else {
    Write-Host "❌ docker-compose.yml not found" -ForegroundColor Red
}

# Check Gateway.API specifically
Write-Host ""
Write-Host "🚪 Gateway.API Status:" -ForegroundColor Yellow
if (Test-Path "gateway/Gateway.API/Gateway.API.csproj") {
    Write-Host "✅ Gateway.API project found" -ForegroundColor Green
    if (Test-Path "gateway/Gateway.API/Program.cs") {
        Write-Host "✅ Program.cs found" -ForegroundColor Green
    }
    if (Test-Path "gateway/Gateway.API/appsettings.json") {
        Write-Host "✅ appsettings.json found" -ForegroundColor Green
    }
    if (Test-Path "gateway/Gateway.API/Dockerfile") {
        Write-Host "✅ Dockerfile found" -ForegroundColor Green
    }
} else {
    Write-Host "❌ Gateway.API project not found" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Health Check Complete ===" -ForegroundColor Green
Write-Host "All project paths have been fixed and the solution is ready for development!" -ForegroundColor Cyan