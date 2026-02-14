# Developer Setup (Client UI)

This guide gets the React client UI running locally on Windows.

## Prerequisites

- Node.js LTS (v14+; v18 or v20 recommended)
- npm (installed with Node.js)
- Git (optional, already used to clone the repo)

### Verify prerequisites

Open PowerShell and run:

```bash
node -v
npm -v
```

If either command is not found, install Node.js LTS from:
https://nodejs.org

Then close and reopen your terminal and re-check the versions.

## Install dependencies

From the repo root:

```bash
cd client
npm install
```

## Run the client (dev mode)

```bash
npm start
```

- The app runs at: http://localhost:3000
- The browser should open automatically.

If PowerShell blocks `npm` with a script policy error or says `npm` is not recognized,
use the Windows command shim instead:

```bash
& "C:\Program Files\nodejs\npm.cmd" start
```

If you see `'"node"' is not recognized`, prepend Node to PATH in the same session:

```bash
$env:Path = "C:\Program Files\nodejs;$env:Path"
& "C:\Program Files\nodejs\npm.cmd" start
```

## API configuration

By default, the client calls `http://localhost:3001/api`.
If your API is hosted elsewhere (for example the C# server on port 3002), create a `.env` file in `client/` with:

```
REACT_APP_API_URL=http://localhost:3002/api
```

Restart `npm start` after changing `.env`.

## Optional: Run the server locally (C#)

If you need data, start the server in a second terminal:

```bash
cd server
dotnet run -- --port 3002
```

Server runs at: http://localhost:3002

Default login (dev):
- Username: admin
- Password: admin

## Troubleshooting

- `npm` not recognized:
  - Install Node.js LTS, reopen terminal, re-run the commands.
- PowerShell script policy blocks `npm`:
  - Use `& "C:\Program Files\nodejs\npm.cmd" start` instead.
- `"node"` not recognized when running `npm.cmd`:
  - Run `$env:Path = "C:\Program Files\nodejs;$env:Path"` (note the leading `$`) and try again.
- Port 3000 in use:
  - Stop the other process or choose a different port when prompted.
- API errors:
  - Confirm the server is running and the `REACT_APP_API_URL` is correct.
