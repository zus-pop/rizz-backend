# Rizz Backend - Microservices Architecture

A modern .NET 8 microservices-based dating application backend with Docker containerization, YARP reverse proxy gateway, and robust database management.

## 🏗️ Architecture Overview

This application consists of **9 microservices** orchestrated through Docker Compose with Microsoft YARP reverse proxy:

### Core Services
- **Gateway API** (Port 5000) - YARP reverse proxy with unified API routing and Swagger UI
- **AI Insights Service** (Port 5001) - AI-powered analytics, insights, and recommendations
- **AuthService.API** (Port 5002) - Authentication, JWT management, and phone verification
- **MatchService.API** (Port 5003) - Swipe functionality, matching algorithm, and match management
- **MessagingService.API** (Port 5004) - Real-time messaging and chat management
- **ModerationService.API** (Port 5005) - Content moderation and safety features
- **PurchaseService.API** (Port 5006) - In-app purchases, subscriptions, and payment processing
- **PushService.API** (Port 5007) - Device token management and push notification delivery
- **UserService.API** (Port 5008) - User profiles, spatial data (PostGIS), and user management
- **NotificationService.API** (Port 5009) - Push notifications and email notifications

### Infrastructure
- **PostgreSQL** (Port 5432) - Database with separate schemas per service
- **RabbitMQ** (Port 5672/15672) - Message broker for inter-service communication

## 🚀 Quick Start

> **✅ Current Status**: All 9 microservices are fully operational with proper YARP Gateway routing. All endpoints return 200 OK responses through the unified gateway.

### Prerequisites

- **Docker Desktop** - [Download here](https://www.docker.com/products/docker-desktop)
- **Git** - For cloning the repository
- **.NET 8 SDK** (optional, for local development)

### 1. Clone and Start

```bash
# Clone the repository
git clone https://github.com/zus-pop/rizz-backend.git
cd rizz-backend

# Start all services
docker compose up -d

# View logs (optional)
docker compose logs -f
```

### 2. Verify Services

Check that all services are healthy:

```bash
# Check service status
docker compose ps

# Test health endpoints through YARP Gateway
curl http://localhost:5000/health                    # Gateway
curl http://localhost:5000/api/aiinsights/health     # AI Insights
curl http://localhost:5000/api/auth/health           # Auth Service  
curl http://localhost:5000/api/swipes/health         # Match Service
curl http://localhost:5000/api/users/health          # User Service
curl http://localhost:5000/api/messages/health       # Messaging Service
curl http://localhost:5000/api/notification/health   # Notification Service
curl http://localhost:5000/api/devicetokens/health   # Push Service
curl http://localhost:5000/api/purchases/health      # Purchase Service
curl http://localhost:5000/api/reports/health        # Moderation Service
curl http://localhost:5000/api/blocks/health         # Moderation Service
```

### 3. Access the APIs

- **🌟 Unified Swagger UI**: [http://localhost:5000](http://localhost:5000)
- **YARP Gateway Routes**: [http://localhost:5000/swagger](http://localhost:5000/swagger)
- **RabbitMQ Management**: [http://localhost:15672](http://localhost:15672) (guest/guest)

### ⚡ Gateway Routing Features
- **Unified API Access**: All 9 microservices accessible through single gateway
- **Multiple Controllers**: UserService supports `/api/users`, `/api/photos`, `/api/profiles`
- **Multi-Controller Services**: ModerationService provides `/api/reports` and `/api/blocks`
- **Health Monitoring**: YARP active health checks for all service endpoints
- **Auto-Discovery**: Swagger JSON aggregation from all services

## 📚 API Endpoints

All APIs are accessible through the YARP Gateway at `http://localhost:5000`:

### Authentication Service (`/api/auth`)
```
POST /api/auth/login           # User login
POST /api/auth/register        # User registration
POST /api/auth/phone-verify    # Phone verification
GET  /api/auth/health          # Service health check
```

### User Service (`/api/users`, `/api/photos`, `/api/profiles`) - PostGIS Spatial Support
```
GET  /api/users               # Get user profiles
POST /api/users               # Create user profile
PUT  /api/users/{id}          # Update user profile
GET  /api/users/nearby        # Find nearby users (spatial query)
GET  /api/photos              # Get user photos
POST /api/photos              # Upload user photo
GET  /api/profiles            # Get user profiles
POST /api/profiles            # Create user profile
GET  /health                  # Service health check
```

### Match Service (`/api/swipes`)
```
POST /api/swipes              # Create swipe
GET  /api/swipes              # Get swipes
GET  /api/swipes/matches      # Get matches
GET  /api/swipes/health       # Service health check
```

### Messaging Service (`/api/messages`)
```
GET  /api/messages            # Get messages
POST /api/messages            # Send message
GET  /api/messages/{id}       # Get specific message
GET  /api/messages/health     # Service health check
```

### Notification Service (`/api/notification`)
```
POST /api/notification/send         # Send notification
GET  /api/notification/user/{id}    # Get user notifications
GET  /api/notification/health       # Service health check
```

### Push Service (`/api/devicetokens`)
```
POST /api/devicetokens              # Register device token
POST /api/devicetokens/send         # Send push notification
GET  /health                        # Service health check
```

### Purchase Service (`/api/purchases`)
```
POST /api/purchases                 # Create purchase
GET  /api/purchases                 # Get purchase history
GET  /api/purchases/user/{id}       # Get user purchases
GET  /api/purchases/health          # Service health check
```

### Moderation Service (`/api/reports`, `/api/blocks`)
```
POST /api/reports                   # Report content
GET  /api/reports                   # Get reports
POST /api/blocks                    # Block user
GET  /api/blocks                    # Get blocks
GET  /api/reports/health            # Service health check
GET  /api/blocks/health             # Service health check
```

### AI Insights Service (`/api/aiinsights`)
```
POST /api/aiinsights/analyze        # AI analysis
GET  /api/aiinsights                # Get AI insights
GET  /api/aiinsights/{userId}       # Get user insights
GET  /api/aiinsights/health         # Service health check
```

## 🔧 Development Setup

### Local Development (Without Docker)

1. **Start Infrastructure Only**:
   ```bash
   # Start PostgreSQL and RabbitMQ only
   docker compose up postgres rabbitmq -d
   ```

2. **Update Connection Strings** (use `localhost` instead of container names):
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database={service}_db;Username=;Password="
     },
     "RabbitMQ": {
       "Host": "localhost",
       "User": "guest", 
       "Pass": "guest"
     }
   }
   ```

3. **Run Services Individually**:
   ```bash
   # Terminal 1 - YARP Gateway
   cd gateway/Gateway.API
   dotnet run

   # Terminal 2 - User Service (PostGIS enabled)
   cd services/UserService.API
   dotnet run

   # Terminal 3 - Auth Service  
   cd services/AuthService.API
   dotnet run

   # Terminal 4 - Match Service
   cd services/MatchService.API
   dotnet run

   # Terminal 5 - Messaging Service
   cd services/MessagingService.API
   dotnet run

   # Additional services...
   cd services/NotificationService.API && dotnet run
   cd services/PushService.API && dotnet run
   cd services/PurchaseService.API && dotnet run
   cd services/ModerationService.API && dotnet run
   cd services/AiInsightsService.API && dotnet run
   ```

### Building Individual Services

```bash
# Build specific service
docker compose build userservice

# Build and restart service
docker compose up userservice --build -d

# View service logs
docker logs userservice -f
```

## 🗄️ Database Management

### Database Schemas

Each service maintains its own database with specific functionality:
- `user_db` - User profiles, spatial data (PostGIS), preferences, and photos
- `auth_db` - User authentication, JWT tokens, and phone verification
- `match_db` - Swipes, matches, and user interactions
- `messaging_db` - Chat conversations and messages
- `notification_db` - Notification history and templates
- `push_db` - Device tokens and push notification logs
- `purchase_db` - In-app purchases, subscriptions, and payment records
- `moderation_db` - Content reports and moderation decisions
- `ai_insights_db` - AI analytics, insights, and user behavior patterns

### PostGIS Spatial Support

The UserService includes PostGIS extension for location-based features:
```sql
-- PostGIS is automatically enabled in user_db
SELECT * FROM spatial_ref_sys LIMIT 1;

-- Example spatial queries
SELECT * FROM users 
WHERE ST_DWithin(location, ST_Point(-74.006, 40.7128), 1000); -- 1km radius
```

### Migrations

Migrations are automatically applied on service startup. To create new migrations:

```bash
# UserService migrations (PostGIS enabled)
cd services/UserService.API
dotnet ef migrations add MigrationName
dotnet ef database update

# AuthService migrations
cd services/AuthService.API
dotnet ef migrations add MigrationName
dotnet ef database update

# Match Service migrations
cd services/MatchService.API  
dotnet ef migrations add MigrationName
dotnet ef database update

# Other services follow the same pattern
cd services/{ServiceName}.API
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Database Access

```bash
# Connect to PostgreSQL
docker exec -it postgres psql -U postgres

# List databases
\l

# Connect to specific database
\c user_db

# List tables
\dt

# Check PostGIS extension (UserService)
\c user_db
SELECT PostGIS_Version();
```

## 🐳 Docker Commands

### Service Management

```bash
# Start all services
docker compose up -d

# Stop all services
docker compose down

# Restart specific service
docker compose restart userservice

# View logs
docker compose logs -f [service-name]

# Scale services (if needed)
docker compose up -d --scale userservice=2
```

### Cleanup

```bash
# Stop and remove containers
docker compose down

# Remove containers and volumes
docker compose down -v

# Remove containers, volumes, and images
docker compose down -v --rmi all
```

## 🔒 Environment Configuration

Key environment variables (configured in `.env`):

```env
# Database
POSTGRES_USER=
POSTGRES_PASSWORD=
POSTGRES_DB=dating_app

# Service Ports
GATEWAY_PORT=5000
AIINSIGHTS_PORT=5001  
AUTHSERVICE_PORT=5002
MATCHSERVICE_PORT=5003
MESSAGINGSERVICE_PORT=5004
MODERATIONSERVICE_PORT=5005
PURCHASESERVICE_PORT=5006
PUSHSERVICE_PORT=5007
USERSERVICE_PORT=5008
NOTIFICATIONSERVICE_PORT=5009

# JWT Configuration
JWT_KEY=your-secret-key
JWT_ISSUER=AuthService
JWT_AUDIENCE=DatingApp

# RabbitMQ
RABBITMQ_DEFAULT_USER=guest
RABBITMQ_DEFAULT_PASS=guest

# PostGIS (UserService)
ENABLE_POSTGIS=true
```

## 🏥 Health Monitoring

### Health Check Endpoints

Each service provides health monitoring through the YARP Gateway:

```bash
# Gateway health
curl http://localhost:5000/health

# Service health checks (routed through Gateway)
curl http://localhost:5000/api/users/health          # User Service
curl http://localhost:5000/api/auth/health           # Auth Service
curl http://localhost:5000/api/swipes/health         # Match Service
curl http://localhost:5000/api/messages/health       # Messaging Service
curl http://localhost:5000/api/notification/health   # Notification Service
curl http://localhost:5000/api/devicetokens/health   # Push Service
curl http://localhost:5000/api/purchases/health      # Purchase Service
curl http://localhost:5000/api/reports/health        # Moderation Service
curl http://localhost:5000/api/aiinsights/health     # AI Insights Service

# Direct service access (bypassing Gateway)
curl http://localhost:5008/health                    # User Service Direct
curl http://localhost:5002/health                    # Auth Service Direct
```

### Monitoring Commands

```bash
# Check container health
docker compose ps

# Monitor resource usage
docker stats

# Check service dependencies
docker compose config
```

## 🛠️ Troubleshooting

### Common Issues

1. **Port Conflicts**:
   ```bash
   # Check port usage
   netstat -ano | findstr :5000
   
   # Change ports in .env file
   GATEWAY_PORT=5010
   ```

2. **Database Connection Issues**:
   ```bash
   # Check PostgreSQL logs
   docker logs postgres
   
   # Verify database is healthy
   docker compose ps postgres
   ```

3. **Service Not Starting**:
   ```bash
   # Check service logs
   docker logs [service-name]
   
   # Rebuild service
   docker compose build [service-name]
   docker compose up [service-name] -d
   ```

4. **Gateway Routing 404 Errors**:
   ```bash
   # Verify correct endpoint paths
   curl http://localhost:5000/api/users     # ✅ Correct
   curl http://localhost:5000/user/api      # ❌ Old format
   
   # Check YARP health monitoring
   docker logs gateway | grep -i health
   
   # Rebuild gateway with latest config
   docker compose build gateway
   docker compose up gateway -d
   ```

5. **Migration Errors**:
   ```bash
   # Reset database (CAUTION: Data loss)
   docker compose down -v
   docker compose up -d
   ```

### Logs and Debugging

```bash
# Follow all logs
docker compose logs -f

# Service-specific logs
docker compose logs -f authservice

# Last 100 log lines
docker compose logs --tail=100 authservice
```

## 🧪 Testing

### API Testing with cURL

```bash
# Test YARP Gateway routing
curl -X GET "http://localhost:5000/api/auth/health"
curl -X GET "http://localhost:5000/api/users/health"  
curl -X GET "http://localhost:5000/api/swipes/health"

# Test direct service access
curl -X GET "http://localhost:5002/health"       # Auth Service
curl -X GET "http://localhost:5008/health"       # User Service
curl -X GET "http://localhost:5003/health"       # Match Service

# Test spatial queries (UserService)
curl -X GET "http://localhost:5000/api/users/nearby?lat=40.7128&lon=-74.0060&radius=1000"

# Test different controllers in UserService
curl -X GET "http://localhost:5000/api/users"
curl -X GET "http://localhost:5000/api/photos"
curl -X GET "http://localhost:5000/api/profiles"
```

### Integration Tests

```bash
# Run comprehensive tests
.\scripts\testing\test-all-services.ps1

# Test specific endpoints
.\scripts\testing\test-endpoints.ps1
```

## 🚀 Production Deployment

### Environment-Specific Configuration

1. **Update connection strings** for production databases
2. **Configure proper JWT secrets** (not default values)
3. **Set up SSL/TLS certificates** for HTTPS
4. **Configure container orchestration** (Kubernetes, Docker Swarm)
5. **Set up monitoring and logging** (ELK Stack, Prometheus)
6. **Configure PostGIS** for production spatial queries
7. **Set up YARP load balancing** for high availability

### Security Considerations

- Change default database passwords
- Use proper JWT signing keys
- Configure CORS policies appropriately  
- Implement rate limiting in YARP Gateway
- Set up container security scanning
- Secure PostGIS spatial data access
- Configure RabbitMQ authentication

### YARP Configuration

The Gateway uses Microsoft YARP for reverse proxy functionality:
- **Controller-based routing**: Routes match actual controller names (`/api/users/*`, `/api/swipes/*`, etc.)
- **Multi-controller support**: Services with multiple controllers get separate routes
- **Health checks**: Integrated health monitoring for all service endpoints (`/health`)
- **Load balancing**: Configurable for multiple service instances
- **Circuit breaker**: Automatic failover for unhealthy services
- **Path preservation**: Full API paths are preserved through routing transforms

#### Current Routing Mappings:
- **UserService**: `/api/users/*`, `/api/photos/*`, `/api/profiles/*` → `userservice:8080`
- **AuthService**: `/api/auth/*` → `authservice:8080`
- **MatchService**: `/api/swipes/*` → `matchservice:8080`
- **MessagingService**: `/api/messages/*` → `messagingservice:8080`
- **ModerationService**: `/api/reports/*`, `/api/blocks/*` → `moderationservice:8080`
- **PurchaseService**: `/api/purchases/*` → `purchaseservice:8080`
- **PushService**: `/api/devicetokens/*` → `pushservice:8080`
- **NotificationService**: `/api/notification/*` → `notificationservice:8080`
- **AiInsightsService**: `/api/aiinsights/*` → `aiinsights:8080`

## 📝 Contributing

1. Fork the repository
2. Create a feature branch
3. Follow existing code patterns
4. Add tests for new functionality
5. Update this README if needed
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 🤝 Support

For issues and questions:
- Create an issue in the GitHub repository
- Check the troubleshooting section above
- Review service logs for error details
