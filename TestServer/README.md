# Test Server

A C# console application that provides a REST API for managing test requirements and test cases, backed by an SQLite database with thread-safe semaphore protection.

## Features

- **SQLite Database**: Lightweight, file-based database storage
- **Thread-Safe Access**: Semaphore-protected database operations for concurrent requests
- **REST API**: Full CRUD operations via HTTP endpoints
- **Database Schema**:
  - Users (Id, Name)
  - Requirements (Id, Text)
  - TestCases (Id, Text)
  - RequirementTestCases (many-to-many mapping)

## Building and Running

### Prerequisites
- .NET 10.0 SDK or later

### Build
```bash
cd TestServer
dotnet build
```

### Run
```bash
dotnet run
```

The server will start on `http://localhost:5000` (or as configured in `appsettings.json`).

## API Endpoints

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create a new user
  ```json
  {"name": "John Doe"}
  ```
- `PUT /api/users/{id}` - Update user
  ```json
  {"name": "Updated Name"}
  ```
- `DELETE /api/users/{id}` - Delete user

### Requirements
- `GET /api/requirements` - Get all requirements
- `GET /api/requirements/{id}` - Get requirement by ID
- `POST /api/requirements` - Create a new requirement
  ```json
  {"text": "User must be able to login"}
  ```
- `PUT /api/requirements/{id}` - Update requirement
  ```json
  {"text": "Updated requirement text"}
  ```
- `DELETE /api/requirements/{id}` - Delete requirement

### Test Cases
- `GET /api/testcases` - Get all test cases
- `GET /api/testcases/{id}` - Get test case by ID
- `POST /api/testcases` - Create a new test case
  ```json
  {"text": "Test login with valid credentials"}
  ```
- `PUT /api/testcases/{id}` - Update test case
  ```json
  {"text": "Updated test case text"}
  ```
- `DELETE /api/testcases/{id}` - Delete test case

### Requirement-TestCase Mappings
- `GET /api/mappings` - Get all requirement-testcase mappings
- `GET /api/requirements/{id}/testcases` - Get all test cases for a requirement
- `GET /api/testcases/{id}/requirements` - Get all requirements for a test case
- `POST /api/mappings` - Create a mapping between requirement and test case
  ```json
  {"requirementId": 1, "testCaseId": 1}
  ```
- `DELETE /api/mappings` - Remove a mapping
  ```json
  {"requirementId": 1, "testCaseId": 1}
  ```

## Examples

### Create a User
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"John Doe"}'
```

### Create a Requirement
```bash
curl -X POST http://localhost:5000/api/requirements \
  -H "Content-Type: application/json" \
  -d '{"text":"User must be able to login"}'
```

### Create a Test Case
```bash
curl -X POST http://localhost:5000/api/testcases \
  -H "Content-Type: application/json" \
  -d '{"text":"Test login with valid credentials"}'
```

### Map a Test Case to a Requirement
```bash
curl -X POST http://localhost:5000/api/mappings \
  -H "Content-Type: application/json" \
  -d '{"requirementId":1,"testCaseId":1}'
```

### Get All Test Cases for a Requirement
```bash
curl http://localhost:5000/api/requirements/1/testcases
```

## Database

The SQLite database file (`testdb.sqlite`) is created automatically in the application's working directory on first run. The database schema is initialized with all required tables:

- **Users**: Stores user information
- **Requirements**: Stores software requirements
- **TestCases**: Stores test cases
- **RequirementTestCases**: Many-to-many mapping table with foreign key constraints

## Thread Safety

All database operations are protected by a `SemaphoreSlim` to ensure thread-safe access in a concurrent environment. This allows the server to handle multiple simultaneous requests without database conflicts.
