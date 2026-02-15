# Developer Setup Guide

This guide explains how to run the TestDB application locally with the server (C# .NET) and client (React) using 5000-series ports.

---

## Server (C# .NET)

### Prerequisites

- .NET SDK 8.0 or higher

### Verify .NET

```bash
dotnet --version
```

### Install server dependencies

```bash
cd server
dotnet restore
```

### Run the server (recommended ports)

```bash
cd server
dotnet run -- --port 5001 --cors-origin "http://localhost:5000"
```

Server runs at: http://localhost:5001

### Optional dev auth bypass

To work on the UI without logging in:

```bash
$env:DEV_BYPASS_AUTH = "true"
dotnet run -- --port 5001 --cors-origin "http://localhost:5000"
```

### Default login (dev)

- Username: admin
- Password: admin

### Logging

All requests and auth events are written to server/events.log.

---

## Client (React)

### Prerequisites

- Node.js LTS (v18 or v20 recommended)
- npm (installed with Node.js)

### Verify Node.js and npm

```bash
node -v
npm -v
```

### Install client dependencies

```bash
cd client
npm install
```

### Configure API URL

Create or update client/.env:

```
REACT_APP_API_URL=http://localhost:5001/api
```

Restart `npm start` after changing `.env` (React reads env vars only at startup).

### Run the client (recommended port)

```powershell
cd client
$env:REACT_APP_API_URL = "http://localhost:5001/api"
$env:PORT = "5000"
$env:Path = "C:\Program Files\nodejs;$env:Path"
& "C:\Program Files\nodejs\npm.cmd" start
```

#### Optional: use the startup script

```powershell
cd client
./ClientStartup.ps1
```

If PowerShell blocks scripts, use the CMD version instead:

```powershell
cd client
./ClientStartup.cmd
```

Client runs at: http://localhost:5000

---

## Troubleshooting

- npm not recognized or blocked by PowerShell
	- Use `& "C:\Program Files\nodejs\npm.cmd" start`
	- If `"node"` is not recognized, run `$env:Path = "C:\Program Files\nodejs;$env:Path"` first
- API errors in the UI
	- Confirm the server is running on port 5001
	- Confirm client/.env is `REACT_APP_API_URL=http://localhost:5001/api`
	- Confirm server started with `--cors-origin "http://localhost:5000"`
	- If the UI still shows a 3002 URL, stop and restart the client dev server
- Port already in use
	- Stop the process on that port or pick another 5000-series port and update both server and client
