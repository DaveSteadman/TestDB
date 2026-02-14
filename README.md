# TestDB

A comprehensive multi-user database system for managing software requirements and test cases with a modern web interface.

## 🏗️ Architecture

This project is split into two main components:

### Server (`/server`)
- C# ASP.NET Core 8.0 Web API console application
- SQLite database with Entity Framework Core
- JWT authentication
- RESTful API for requirements, test cases, and mappings
- Multi-user support
- Command-line parameter configuration

### Client (`/client`)
- React-based web interface
- Modern, polished UI with gradient design
- Responsive layout
- Full CRUD operations for requirements and tests
- Visual requirement-to-test mapping

## 📊 Database Schema

The system includes four main tables:

1. **users** - User accounts with authentication
2. **requirements** - Software requirements with priority and status tracking
3. **test_cases** - Test cases with steps and expected results
4. **requirement_test_mapping** - Many-to-many relationship between requirements and tests

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK or later
- Node.js (v14 or higher) for the client
- npm or yarn

### Setup Server

```bash
cd server
dotnet build       # Build the application
dotnet run         # Start server on port 3001
```

The database will be automatically created and initialized on first run with a default admin user.

Default admin credentials:
- Username: `admin`
- Password: `admin123`

### Setup Client

```bash
cd client
npm install
npm start        # Start client on port 3000
```

The client will open automatically in your browser at `http://localhost:3000`.

## 📖 Usage

1. **Login** - Use the admin credentials or register a new account
2. **Dashboard** - View statistics and overview
3. **Requirements** - Create and manage software requirements with priorities and statuses
4. **Test Cases** - Create and manage test cases with steps and expected results
5. **Mappings** - Link requirements to their corresponding test cases

## 🔧 Configuration

### Server Configuration
- Port: Set via `--port` command-line parameter or `Server:Port` in appsettings.json (default: 3001)
- JWT Secret: Set via `--jwt-secret` parameter or `Jwt:Secret` in appsettings.json (change in production!)
- CORS Origin: Set via `--cors-origin` parameter or `Cors:AllowedOrigin` in appsettings.json
- Environment: Set via `--environment` parameter or ASPNETCORE_ENVIRONMENT variable

Example with command-line parameters:
```bash
dotnet run --port 3002 --jwt-secret "your-secret" --cors-origin "https://example.com"
```

### Client Configuration
- API URL: Set via `REACT_APP_API_URL` in `.env` file (default: http://localhost:3001/api)

## 📁 Project Structure

```
TestDB/
├── server/              # Backend server (C# ASP.NET Core)
│   ├── Controllers/     # API controllers
│   ├── Models/          # Data models
│   ├── Data/            # EF Core DbContext
│   ├── Services/        # Business logic services
│   ├── DTOs/            # Data transfer objects
│   ├── Program.cs       # Application entry point
│   ├── appsettings.json # Configuration
│   └── README.md        # Server documentation
├── client/              # Frontend client
│   ├── public/          # Static files
│   ├── src/             # React source code
│   │   ├── components/  # React components
│   │   ├── services/    # API services
│   │   └── styles/      # CSS styles
│   ├── package.json     # Client dependencies
│   └── README.md        # Client documentation
└── README.md            # This file
```

## 🎨 Features

### Requirements Management
- Create, edit, and delete requirements
- Priority levels: Low, Medium, High, Critical
- Status tracking: Draft, Active, Completed, Deprecated
- User attribution

### Test Cases Management
- Create, edit, and delete test cases
- Detailed test steps
- Expected results
- Status tracking: Draft, Active, Passed, Failed, Blocked
- User attribution

### Requirement-Test Mapping
- Visual mapping interface
- Many-to-many relationships
- Easy linking and unlinking

### User Management
- Secure authentication with JWT
- User registration
- Password hashing with bcrypt
- Session management

## 🔒 Security

- Passwords are hashed using BCrypt.Net
- JWT tokens for authentication
- Protected API endpoints
- CORS configured (set via --cors-origin parameter or appsettings.json)

**⚠️ Important Security Notes:**
1. Change the default admin password (`admin123`) immediately after first login
2. Set a strong JWT secret via `--jwt-secret` parameter or appsettings.json in production
3. Configure CORS origin to restrict API access to your client domain
4. Use HTTPS in production environments
5. Consider adding rate limiting for production deployments to prevent abuse

## 📚 API Documentation

See `/server/README.md` for detailed API endpoint documentation.

## 🛠️ Development

### Server Development
```bash
cd server
dotnet watch run  # Run with hot reload
```

### Client Development
```bash
cd client
npm start  # Run with hot reload
```

## 📝 License

MIT