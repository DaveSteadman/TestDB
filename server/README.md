# TestDB Server - C# Console Application

Multi-user database server for managing software requirements and test cases, built with ASP.NET Core Web API.

## Features

- User authentication with JWT tokens
- RESTful API for requirements management
- RESTful API for test cases management
- Requirement-to-test case mapping
- SQLite database backend with Entity Framework Core
- Command-line parameter support for flexible configuration

## Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later

## Database Schema

### Tables

1. **users** - User accounts with authentication
2. **requirements** - Software requirements with priority and status
3. **test_cases** - Test cases with steps and expected results
4. **requirement_test_mapping** - Many-to-many relationship between requirements and tests

## Installation

### Build the Application

```bash
cd server
dotnet build
```

### Run the Application

```bash
dotnet run
```

The database will be automatically created and initialized on first run with a default admin user:
- Username: `admin`
- Password: `admin123`

**⚠️ SECURITY WARNING**: Change the default admin password immediately after first login!

## Configuration

### Using appsettings.json

Edit `appsettings.json` to configure:
- Server port (default: 3001)
- JWT secret key
- CORS allowed origin
- Database connection string

### Using Command-Line Parameters

Command-line parameters override settings in `appsettings.json`:

```bash
# Set port
dotnet run -- --port 3002
dotnet run -- -p 3002

# Set environment
dotnet run -- --environment Production
dotnet run -- -e Production

# Set JWT secret
dotnet run -- --jwt-secret "your-custom-secret-key"

# Set CORS origin
dotnet run -- --cors-origin "http://localhost:5000"

# Combine multiple parameters
dotnet run -- --port 3002 --environment Production --jwt-secret "secure-key" --cors-origin "https://example.com"
```

### Using Environment Variables

You can also use environment variables:

```bash
# Windows (PowerShell)
$env:Jwt__Secret = "your-secret-key"
dotnet run

# Linux/macOS
export Jwt__Secret="your-secret-key"
dotnet run
```

### Development Auth Bypass (Optional)

To work on the UI without logging in, you can bypass auth on the server:

```bash
# Windows (PowerShell)
$env:DEV_BYPASS_AUTH = "true"
dotnet run -- --port 5001

# Linux/macOS
export DEV_BYPASS_AUTH=true
dotnet run -- --port 5001
```

This sets a temporary dev identity (username: admin) for every request that is not authenticated.

You can also set the listening URL explicitly:

```bash
# Windows (PowerShell)
$env:ASPNETCORE_URLS = "http://localhost:3002"
dotnet run

# Linux/macOS
export ASPNETCORE_URLS="http://localhost:3002"
dotnet run
```

## API Endpoints

### Authentication

- `POST /api/auth/login` - Login with username and password
- `POST /api/auth/register` - Register a new user

### Requirements (Authentication Required)

- `GET /api/requirements` - Get all requirements
- `GET /api/requirements/{id}` - Get a specific requirement
- `POST /api/requirements` - Create a new requirement
- `PUT /api/requirements/{id}` - Update a requirement
- `DELETE /api/requirements/{id}` - Delete a requirement
- `GET /api/requirements/{id}/test-cases` - Get all test cases linked to a requirement

### Test Cases (Authentication Required)

- `GET /api/test-cases` - Get all test cases
- `GET /api/test-cases/{id}` - Get a specific test case
- `POST /api/test-cases` - Create a new test case
- `PUT /api/test-cases/{id}` - Update a test case
- `DELETE /api/test-cases/{id}` - Delete a test case
- `GET /api/test-cases/{id}/requirements` - Get all requirements linked to a test case

### Mappings (Authentication Required)

- `GET /api/mappings` - Get all requirement-test mappings
- `POST /api/mappings` - Create a new mapping
  - Body: `{ "requirementId": 1, "testCaseId": 1 }`
- `DELETE /api/mappings/{id}` - Delete a mapping

### Health Check

- `GET /api/health` - Check server status

## Authentication

All API endpoints (except `/api/auth/login`, `/api/auth/register`, and `/api/health`) require authentication.

Include the JWT token in the Authorization header:

```
Authorization: Bearer <token>
```

## Example Request

```bash
# Login
curl -X POST http://localhost:3001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'

# Create a requirement (with token)
curl -X POST http://localhost:3001/api/requirements \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <your-token>" \
  -d '{"title":"User Login","description":"User should be able to login","priority":"High","status":"Active"}'
```

## Development

### Run in Development Mode

```bash
dotnet run --environment Development
```

### Watch for Changes

```bash
dotnet watch run
```

### Restore Dependencies

```bash
dotnet restore
```

## Production Deployment

For production deployment:

1. Set a strong JWT secret using environment variable or command-line parameter
2. Configure CORS to match your frontend domain
3. Consider using a more robust database (the code can be adapted for PostgreSQL, SQL Server, etc.)
4. Build in Release mode:

```bash
dotnet build -c Release
dotnet run -c Release --jwt-secret "strong-random-secret-key" --cors-origin "https://yourdomain.com"
```

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQLite with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Password Hashing**: BCrypt
- **Architecture**: MVC with Controllers, Services, and Data Access Layer

## Project Structure

```
server/
├── Program.cs                          # Application entry point
├── TestDB.Server.csproj                # Project file
├── appsettings.json                    # Configuration
├── appsettings.Development.json        # Development configuration
├── Data/
│   ├── TestDbContext.cs               # EF Core DbContext
│   └── DbInitializer.cs               # Database initialization
├── Models/
│   ├── User.cs                        # User entity
│   ├── Requirement.cs                 # Requirement entity
│   ├── TestCase.cs                    # TestCase entity
│   └── RequirementTestMapping.cs      # Mapping entity
├── DTOs/
│   ├── LoginDto.cs                    # Login DTO
│   ├── RegisterDto.cs                 # Register DTO
│   ├── RequirementDto.cs              # Requirement DTOs
│   └── TestCaseDto.cs                 # TestCase DTOs
├── Services/
│   ├── IAuthService.cs                # Auth service interface
│   └── AuthService.cs                 # Auth service implementation
└── Controllers/
    ├── AuthController.cs              # Authentication endpoints
    ├── RequirementsController.cs      # Requirements CRUD
    ├── TestCasesController.cs         # Test cases CRUD
    ├── MappingsController.cs          # Mapping management
    └── HealthController.cs            # Health check
```
