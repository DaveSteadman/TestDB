# TestDB Server

Multi-user database server for managing software requirements and test cases.

## Features

- User authentication with JWT tokens
- RESTful API for requirements management
- RESTful API for test cases management
- Requirement-to-test case mapping
- SQLite database backend

## Database Schema

### Tables

1. **users** - User accounts with authentication
2. **requirements** - Software requirements with priority and status
3. **test_cases** - Test cases with steps and expected results
4. **requirement_test_mapping** - Many-to-many relationship between requirements and tests

## Installation

```bash
npm install
```

## Initialize Database

```bash
npm run init-db
```

This creates the database with all tables and a default admin user:
- Username: `admin`
- Password: `admin123`

## Running the Server

```bash
# Production
npm start

# Development (with auto-reload)
npm run dev
```

The server runs on port 3001 by default.

## API Endpoints

### Authentication

- `POST /api/auth/login` - Login with username and password
- `POST /api/auth/register` - Register a new user

### Requirements

- `GET /api/requirements` - Get all requirements
- `GET /api/requirements/:id` - Get a specific requirement
- `POST /api/requirements` - Create a new requirement
- `PUT /api/requirements/:id` - Update a requirement
- `DELETE /api/requirements/:id` - Delete a requirement
- `GET /api/requirements/:id/test-cases` - Get all test cases linked to a requirement

### Test Cases

- `GET /api/test-cases` - Get all test cases
- `GET /api/test-cases/:id` - Get a specific test case
- `POST /api/test-cases` - Create a new test case
- `PUT /api/test-cases/:id` - Update a test case
- `DELETE /api/test-cases/:id` - Delete a test case
- `GET /api/test-cases/:id/requirements` - Get all requirements linked to a test case

### Mappings

- `GET /api/mappings` - Get all requirement-test mappings
- `POST /api/mappings` - Create a new mapping
- `DELETE /api/mappings/:id` - Delete a mapping

### Health Check

- `GET /api/health` - Check server status

## Authentication

All API endpoints (except `/api/auth/login` and `/api/auth/register`) require authentication.

Include the JWT token in the Authorization header:

```
Authorization: Bearer <token>
```

## Environment Variables

- `PORT` - Server port (default: 3001)
- `JWT_SECRET` - Secret key for JWT tokens (change in production!)
