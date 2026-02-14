# Client Event Logging

The TestDB client now includes comprehensive event logging to help debug connection and API issues.

## Features

- **Automatic Logging**: All API requests, responses, and errors are automatically logged
- **Browser Storage**: Logs are stored in the browser's localStorage
- **Download Capability**: Click the "📋 Logs" button in the navbar to download all client logs as a file
- **Console Output**: All events are also logged to the browser console with color coding

## What Gets Logged

### Connection Events
- `CONNECTION_ATTEMPT`: When the client tries to connect to the server
- `CONNECTION_SUCCESS`: When connection is successful
- `CONNECTION_FAILURE`: When connection fails (includes error details)
- `CONNECTION_ERROR`: Additional error context (timeouts, network errors)

### Authentication Events
- `AUTH_ATTEMPT`: Login attempt with username
- `AUTH_SUCCESS`: Successful login
- `AUTH_FAILURE`: Failed login with error details
- `AUTH_REGISTER`: Registration attempt
- `AUTH_LOGOUT`: User logout

### API Events
- `API_INIT`: API service initialization with base URL
- `API_REQUEST`: Every API request with method and URL
- `API_RESPONSE`: Successful responses with status code and duration
- `API_ERROR`: Failed requests with error details
- `API_TIMEOUT`: Request timeouts

### Application Events
- `CLIENT_START`: Application startup
- `APP_INIT`: Application initialization with configuration
- `APP_AUTH_SKIP`: When authentication is skipped
- `APP_AUTH_RESTORED`: When authentication is restored from storage
- `APP_LOGIN_SUCCESS`: Successful login flow

## How to Use

### View Logs in Console
Open your browser's Developer Tools (F12) and check the Console tab. All events will be logged there.

### Download Logs
1. Click the "📋 Logs" button in the navigation bar
2. A file named `ClientEvents_YYYY-MM-DDTHH-MM-SS.log` will be downloaded

### Clear Logs
Logs are automatically limited to the last 1000 entries to prevent memory issues.

## Example Log Entries

```
2026-02-14T23:30:00.000Z CLIENT_START TestDB client initialized {"apiUrl":"http://localhost:3001/api","timestamp":"2026-02-14T23:30:00.000Z"}
2026-02-14T23:30:00.100Z API_INIT Initializing API with base URL: http://localhost:3001/api
2026-02-14T23:30:00.200Z CONNECTION_ATTEMPT Attempting to connect to http://localhost:3001/api
2026-02-14T23:30:00.350Z API_REQUEST GET /health
2026-02-14T23:30:00.450Z API_RESPONSE GET /health -> 200 (100ms)
2026-02-14T23:30:00.500Z CONNECTION_SUCCESS Successfully connected to http://localhost:3001/api
2026-02-14T23:30:05.000Z AUTH_ATTEMPT Login attempt for user: admin
2026-02-14T23:30:05.250Z API_REQUEST POST /auth/login
2026-02-14T23:30:05.500Z API_RESPONSE POST /auth/login -> 200 (250ms)
2026-02-14T23:30:05.550Z AUTH_SUCCESS Login successful for user: admin
```

## Troubleshooting

### Connection Issues
If you see `CONNECTION_FAILURE` events, check:
1. Is the server running?
2. Is the server on the correct port (3001 by default)?
3. Is CORS configured correctly on the server?
4. Check the error details in the log entry

### Slow Performance
If requests are slow, check the duration in `API_RESPONSE` events. The time is shown in milliseconds.

### Authentication Problems
Look for `AUTH_FAILURE` events for specific error messages about why login failed.
