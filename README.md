# TestDB

A comprehensive multi-user database system for managing software requirements and test cases with a modern web interface.

## 🏗️ Architecture

This project is split into two main components:

### Server (`/server`)
- Node.js/Express backend
- SQLite database
- JWT authentication
- RESTful API for requirements, test cases, and mappings
- Multi-user support

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
- Node.js (v14 or higher)
- npm or yarn

### Setup Server

```bash
cd server
npm install
npm run init-db  # Initialize database with tables and default admin user
npm start        # Start server on port 3001
```

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
- Port: Set via `PORT` environment variable (default: 3001)
- JWT Secret: Set via `JWT_SECRET` environment variable (change in production!)

### Client Configuration
- API URL: Set via `REACT_APP_API_URL` in `.env` file (default: http://localhost:3001/api)

## 📁 Project Structure

```
TestDB/
├── server/              # Backend server
│   ├── server.js        # Main server file
│   ├── init-db.js       # Database initialization
│   ├── package.json     # Server dependencies
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

- Passwords are hashed using bcryptjs
- JWT tokens for authentication
- Protected API endpoints
- CORS configured (set CORS_ORIGIN env variable for production)

**⚠️ Important Security Notes:**
1. Change the default admin password (`admin123`) immediately after first login
2. Set a strong `JWT_SECRET` environment variable in production
3. Configure `CORS_ORIGIN` to restrict API access to your client domain
4. Use HTTPS in production environments

## 📚 API Documentation

See `/server/README.md` for detailed API endpoint documentation.

## 🛠️ Development

### Server Development
```bash
cd server
npm run dev  # Run with nodemon for auto-reload
```

### Client Development
```bash
cd client
npm start  # Run with hot reload
```

## 📝 License

MIT