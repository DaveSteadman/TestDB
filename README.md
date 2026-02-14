# TestDB

A test server application providing a REST API for managing software requirements and test cases.

## Overview

This repository contains a C# console application that implements a test management server with:

- **SQLite Database**: Lightweight database for storing users, requirements, test cases, and their relationships
- **Thread-Safe Access**: Semaphore-protected database operations for concurrent request handling
- **REST API**: Complete CRUD operations via HTTP endpoints
- **Many-to-Many Relationships**: Link test cases to requirements

## Project Structure

- `/TestServer` - The main C# console application

## Quick Start

```bash
cd TestServer
dotnet build
dotnet run
```

The server will start on `http://localhost:5000`.

See the [TestServer README](TestServer/README.md) for detailed API documentation and usage examples.

## Database Schema

- **Users** - User accounts (Id, Name)
- **Requirements** - Software requirements (Id, Text)
- **TestCases** - Test cases (Id, Text)  
- **RequirementTestCases** - Many-to-many mapping of requirements to test cases

## API Endpoints

The REST API provides endpoints for:
- User management (`/api/users`)
- Requirement management (`/api/requirements`)
- Test case management (`/api/testcases`)
- Requirement-TestCase mappings (`/api/mappings`)

For complete API documentation, see the [TestServer README](TestServer/README.md).