# Gateway API - YARP Reverse Proxy with Swagger Aggregation

This is the API Gateway for the Dating App microservices architecture, built with **Microsoft YARP (Yet Another Reverse Proxy)** and featuring Swagger documentation aggregation.

## 🚀 Features

- **YARP Reverse Proxy**: High-performance reverse proxy for microservices
- **Swagger Aggregation**: Unified Swagger UI displaying all microservice APIs
- **JWT Authentication**: Centralized authentication handling
- **Health Checks**: Comprehensive health monitoring for all services
- **CORS Support**: Cross-origin resource sharing configuration
- **Load Balancing**: Automatic request distribution
- **Service Discovery**: Dynamic service endpoint management

## 🔗 Service Routes

The gateway routes requests to the following microservices:

| Service | Route Pattern | Internal Address | Swagger Endpoint |
|---------|---------------|------------------|------------------|
| **AuthService** | `/api/auth/*` | `http://authservice:8080` | `/api-docs/auth.json` |
| **UserService** | `/api/users/*` | `http://userservice:8080` | `/api-docs/user.json` |
| **MatchService** | `/api/matches/*` | `http://matchservice:8080` | `/api-docs/match.json` |
| **MessagingService** | `/api/messages/*` | `http://messagingservice:8080` | `/api-docs/messaging.json` |
| **NotificationService** | `/api/notifications/*` | `http://notificationservice:8080` | `/api-docs/notification.json` |
| **ModerationService** | `/api/moderation/*` | `http://moderationservice:8080` | `/api-docs/moderation.json` |
| **PurchaseService** | `/api/purchases/*` | `http://purchaseservice:8080` | `/api-docs/purchase.json` |
| **PushService** | `/api/push/*` | `http://pushservice:8080` | `/api-docs/push.json` |
| **AiInsightsService** | `/api/ai-insights/*` | `http://aiinsights:8080` | `/api-docs/aiinsights.json` |

## 📊 Health Monitoring

### Health Check Endpoints

- **Basic Health**: `GET /health`
- **Detailed Health**: `GET /health/detailed`
- **Health Dashboard**: `GET /health-ui`
- **Health API**: `GET /health-api`

### Gateway Management

- **Gateway Status**: `GET /api/gateway/status`
- **Route Information**: `GET /api/gateway/routes`

## 🔐 Authentication

The gateway handles JWT authentication centrally. Include the JWT token in requests:

```
Authorization: Bearer {your-jwt-token}
```

### JWT Configuration

```json
{
  "Jwt": {
    "Key": "${JWT_KEY}",
    "Issuer": "${JWT_ISSUER}",
    "Audience": "${JWT_AUDIENCE}"
  }
}
```

## 📚 Swagger Documentation

### Access Points

- **Unified Swagger UI**: `http://localhost:5000/` (Root path)
- **Gateway API Docs**: `/swagger/v1/swagger.json`

### Aggregated Service Documentation

The gateway aggregates Swagger documentation from all microservices:

1. **AI Insights Service**: AI-powered insights and recommendations
2. **Auth Service**: Authentication and authorization
3. **Match Service**: User matching algorithms
4. **Messaging Service**: Real-time messaging between users
5. **Moderation Service**: Content moderation and safety
6. **Purchase Service**: In-app purchases and billing
7. **Push Service**: Firebase push notifications
8. **User Service**: User profile management
9. **Notification Service**: Internal notifications

## 🐳 Docker Configuration

### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["gateway/Gateway.API/Gateway.API.csproj", "gateway/Gateway.API/"]
RUN dotnet restore "./gateway/Gateway.API/Gateway.API.csproj"
COPY . .
WORKDIR "/src/gateway/Gateway.API"
RUN dotnet build "./Gateway.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Gateway.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.API.dll"]
```

### Environment Variables

```bash
# JWT Configuration
JWT_KEY=your-super-secret-jwt-key
JWT_ISSUER=AuthService
JWT_AUDIENCE=DatingApp

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Development
```

## 🛠️ Development

### Running Locally

```bash
# Navigate to gateway directory
cd gateway/Gateway.API

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

### Using Docker Compose

```bash
# From project root
docker-compose up gateway
```

## 🔧 YARP Configuration

The gateway uses YARP configuration in `appsettings.json`:

```json
{
  "ReverseProxy": {
    "Routes": {
      "auth-route": {
        "ClusterId": "auth-cluster",
        "Match": {
          "Path": "/api/auth/{**catch-all}"
        },
        "Transforms": [
          {
            "PathPattern": "/api/auth/{**catch-all}"
          }
        ]
      }
    },
    "Clusters": {
      "auth-cluster": {
        "Destinations": {
          "auth/destination1": {
            "Address": "http://authservice:8080/"
          }
        },
        "HealthCheck": {
          "Active": {
            "Enabled": true,
            "Interval": "00:00:30",
            "Timeout": "00:00:05",
            "Policy": "ConsecutiveFailures",
            "Path": "/health"
          }
        }
      }
    }
  }
}
```

## 🔄 Load Balancing

YARP automatically distributes requests across multiple service instances when available. Health checks ensure only healthy instances receive traffic.

## 🚨 Error Handling

- **Service Unavailable**: Gateway handles service downtime gracefully
- **Timeout Management**: Configurable request timeouts
- **Fallback Responses**: Default responses when services are unavailable
- **Circuit Breaker**: Automatic failure detection and recovery

## 📈 Performance Features

- **Connection Pooling**: Efficient HTTP connection management
- **Request Buffering**: Optimized request/response handling
- **Compression**: Automatic response compression
- **Caching Headers**: Proper HTTP caching support

## 🔍 Monitoring and Observability

### Health Check Features

- **Active Health Checks**: Regular service health monitoring
- **Passive Health Checks**: Automatic failure detection
- **Health Dashboard**: Visual health status monitoring
- **Metrics Collection**: Performance and health metrics

### Logging

- **Structured Logging**: JSON-formatted logs
- **Request Tracing**: Detailed request/response logging
- **Error Tracking**: Comprehensive error logging
- **Performance Metrics**: Request duration and throughput

## 🚀 Production Considerations

### Scaling

- **Horizontal Scaling**: Multiple gateway instances
- **Session Affinity**: Sticky sessions when needed
- **Rate Limiting**: Request throttling capabilities
- **Request Size Limits**: Configurable payload limits

### Security

- **HTTPS Enforcement**: SSL/TLS termination
- **Security Headers**: HSTS, CSP, and other security headers
- **Input Validation**: Request validation and sanitization
- **Authentication Forwarding**: Secure token forwarding

## 🛟 Troubleshooting

### Common Issues

1. **Service Not Reachable**: Check service health endpoints
2. **Authentication Failures**: Verify JWT configuration
3. **Swagger Not Loading**: Check service endpoint connectivity
4. **High Latency**: Review service health and network connectivity

### Debug Endpoints

- `/health/detailed` - Detailed health information
- `/api/gateway/status` - Gateway status information
- `/api/gateway/routes` - Route configuration details

---

**Built with Microsoft YARP and ASP.NET Core 8.0**