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

## API configuration

By default, the client calls `http://localhost:3001/api`.
If your API is hosted elsewhere, create a `.env` file in `client/` with:

```
REACT_APP_API_URL=http://localhost:3001/api
```

Restart `npm start` after changing `.env`.

## Optional: Run the server locally

If you need data, start the server in a second terminal:

```bash
cd server
npm install
npm run init-db
npm start
```

Server runs at: http://localhost:3001

Default login (if you ran `init-db`):
- Username: admin
- Password: admin123

## Troubleshooting

- `npm` not recognized:
  - Install Node.js LTS, reopen terminal, re-run the commands.
- Port 3000 in use:
  - Stop the other process or choose a different port when prompted.
- API errors:
  - Confirm the server is running and the `REACT_APP_API_URL` is correct.
