# Developer Setup Guide

This guide explains how to run the TestDB application locally with the server (C# .NET) and client (React).

## Prerequisites

### Server (C# .NET)
- .NET SDK 6.0 or higher
- SQLite (included with .NET SDK)

### Client (React)
- Node.js LTS (v14+; v18 or v20 recommended)
- npm (installed with Node.js)

### Verify Prerequisites

**Check .NET:**
```bash
dotnet --version
```

**Check Node.js and npm:**
```bash
node -v
npm -v
```

If any command is not found:
- Install .NET SDK from: https://dotnet.microsoft.com/download
- Install Node.js LTS from: https://nodejs.org

Then close and reopen your terminal and re-check the versions.

## Installation

### 1. Install Server Dependencies

```bash
cd server
dotnet restore
```

### 2. Install Client Dependencies

```bash
cd client
npm install
```

## Running the Application

The server and client must both be running for the application to work. Login functionality requires the server to be available.

### Default Ports (No Configuration Needed)

If you don't have port conflicts, you can use the default configuration:

**Terminal 1 - Start the server:**
```bash
cd server
dotnet run
```
Server runs at: `http://localhost:3001`

**Terminal 2 - Start the client:**
```bash
cd client
npm start
```
Client runs at: `http://localhost:3000`

The client will automatically connect to the server at `http://localhost:3001/api`.

### Custom Ports (Avoiding Port Conflicts)

If the default ports are in use, follow these steps to configure custom ports:

#### Step 1: Choose Your Ports

Example configuration:
- Server port: **5001**
- Client port: **5000**

#### Step 2: Configure the Client API URL

Create a `.env` file in the `client/` directory:

**client/.env**
```
REACT_APP_API_URL=http://localhost:5001/api
```

Replace `5001` with your chosen server port.

#### Step 3: Start the Server with Custom Port and CORS

**Windows PowerShell:**
```powershell
cd server
dotnet run -- --port 5001 --cors-origin "http://localhost:5000"
```

**Linux/Mac:**
```bash
cd server
dotnet run -- --port 5001 --cors-origin "http://localhost:5000"
```

Replace `5001` with your chosen server port and `5000` with your chosen client port.

#### Step 4: Start the Client with Custom Port

**Windows PowerShell:**
```powershell
cd client
$env:PORT=5000; npm start
```

**Linux/Mac:**
```bash
cd client
PORT=5000 npm start
```

Replace `5000` with your chosen client port.

### Command-Line Parameters Reference

**Server parameters:**
- `--port <number>` - Set the server port (default: 3001)
- `--cors-origin "<url>"` - Set allowed CORS origin (default: http://localhost:3000)
- Multiple CORS origins: `--cors-origin "http://localhost:3000,http://localhost:5000"`

**Client environment variables:**
- `PORT` - Set the client port (default: 3000)
- `REACT_APP_API_URL` - Set the API URL (configured in `.env` file)

## Default Login Credentials

After the server starts for the first time, it creates a default admin user:

- **Username:** `admin`
- **Password:** `admin`

**Important:** Change these credentials in production environments.

## Troubleshooting

### npm or node Not Recognized (Windows)

If PowerShell blocks `npm` or says it's not recognized:

```powershell
& "C:\Program Files\nodejs\npm.cmd" start
```

If you see `'"node"' is not recognized`, add Node to PATH:

```powershell
$env:Path = "C:\Program Files\nodejs;$env:Path"
& "C:\Program Files\nodejs\npm.cmd" start
```

### Port Already in Use

If you see "port already in use" errors:
1. Stop the other process using that port, OR
2. Use custom ports as described in the "Custom Ports" section above

### Login Not Working / API Errors

**Problem:** Login button doesn't work or returns errors.

**Solution:** Make sure:
1. The server is running (it's **required**, not optional)
2. The client's `.env` file has the correct `REACT_APP_API_URL` matching the server port
3. The server's `--cors-origin` parameter matches the client URL
4. You restarted `npm start` after creating or modifying the `.env` file

**Example with server on port 5001 and client on port 5000:**
- Client `.env` file: `REACT_APP_API_URL=http://localhost:5001/api`
- Server command: `dotnet run -- --port 5001 --cors-origin "http://localhost:5000"`
- Client command: `PORT=5000 npm start` (Linux/Mac) or `$env:PORT=5000; npm start` (Windows)

### CORS Errors

If you see CORS errors in the browser console:
- Verify the server's `--cors-origin` parameter matches the client URL exactly
- Include the protocol (`http://`) and port number
- Restart the server after changing the CORS configuration

### API Connection Errors

If the client can't connect to the API:
- Verify the server is running and accessible
- Check that `REACT_APP_API_URL` in the client's `.env` file matches the server URL
- Restart `npm start` after modifying the `.env` file
