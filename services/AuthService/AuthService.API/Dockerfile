# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared libraries first
COPY shared/ shared/

# Copy the specific service
COPY services/AuthService/AuthService.API/ services/AuthService/AuthService.API/

# Set working directory to the service
WORKDIR /src/services/AuthService/AuthService.API

# Restore and build
RUN dotnet restore "AuthService.API.csproj"
RUN dotnet publish "AuthService.API.csproj" -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.API.dll"]
