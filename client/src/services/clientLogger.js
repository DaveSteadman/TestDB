/**
 * Client-side event logger for debugging connection and API issues
 * Logs events to console and localStorage, with ability to download logs
 */

class ClientLogger {
  constructor() {
    this.logs = [];
    this.maxLogs = 1000; // Keep last 1000 logs in memory
    this.storageKey = 'testdb_client_logs';
    this.loadLogsFromStorage();
  }

  log(eventType, message, data = null) {
    const timestamp = new Date().toISOString();
    const logEntry = {
      timestamp,
      eventType,
      message,
      data,
    };

    // Add to memory
    this.logs.push(logEntry);
    if (this.logs.length > this.maxLogs) {
      this.logs.shift();
    }

    // Save to localStorage
    this.saveLogsToStorage();

    // Console output with color coding
    const consoleMsg = `[${timestamp}] ${eventType}: ${message}`;
    if (eventType.includes('ERROR') || eventType.includes('FAIL')) {
      console.error(consoleMsg, data || '');
    } else if (eventType.includes('WARN')) {
      console.warn(consoleMsg, data || '');
    } else {
      console.log(consoleMsg, data || '');
    }
  }

  loadLogsFromStorage() {
    try {
      const stored = localStorage.getItem(this.storageKey);
      if (stored) {
        this.logs = JSON.parse(stored);
      }
    } catch (error) {
      console.error('Failed to load logs from storage:', error);
    }
  }

  saveLogsToStorage() {
    try {
      localStorage.setItem(this.storageKey, JSON.stringify(this.logs));
    } catch (error) {
      console.error('Failed to save logs to storage:', error);
    }
  }

  getLogs() {
    return this.logs;
  }

  clearLogs() {
    this.logs = [];
    localStorage.removeItem(this.storageKey);
    this.log('LOGGER', 'Logs cleared');
  }

  downloadLogs() {
    const logsText = this.logs
      .map((log) => {
        let line = `${log.timestamp} ${log.eventType} ${log.message}`;
        if (log.data) {
          line += ` ${JSON.stringify(log.data)}`;
        }
        return line;
      })
      .join('\n');

    const blob = new Blob([logsText], { type: 'text/plain' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `ClientEvents_${new Date().toISOString().replace(/[:.]/g, '-')}.log`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
    this.log('LOGGER', 'Logs downloaded');
  }

  // Convenience methods for common event types
  logApiRequest(method, url) {
    this.log('API_REQUEST', `${method} ${url}`);
  }

  logApiResponse(method, url, status, duration) {
    this.log('API_RESPONSE', `${method} ${url} -> ${status} (${duration}ms)`);
  }

  logApiError(method, url, error) {
    const errorMsg = error.response?.data?.error || error.message || 'Unknown error';
    const status = error.response?.status || 'N/A';
    this.log('API_ERROR', `${method} ${url} -> ${status}: ${errorMsg}`, {
      error: error.message,
      status,
      data: error.response?.data,
    });
  }

  logConnectionAttempt(url) {
    this.log('CONNECTION_ATTEMPT', `Attempting to connect to ${url}`);
  }

  logConnectionSuccess(url) {
    this.log('CONNECTION_SUCCESS', `Successfully connected to ${url}`);
  }

  logConnectionFailure(url, error) {
    this.log('CONNECTION_FAILURE', `Failed to connect to ${url}: ${error.message}`, {
      error: error.message,
    });
  }

  logAuthAttempt(username) {
    this.log('AUTH_ATTEMPT', `Login attempt for user: ${username}`);
  }

  logAuthSuccess(username) {
    this.log('AUTH_SUCCESS', `Login successful for user: ${username}`);
  }

  logAuthFailure(username, error) {
    this.log('AUTH_FAILURE', `Login failed for user: ${username}: ${error}`, {
      error,
    });
  }
}

// Create singleton instance
const clientLogger = new ClientLogger();

// Log initial startup
clientLogger.log('CLIENT_START', 'TestDB client initialized', {
  apiUrl: process.env.REACT_APP_API_URL || 'http://localhost:3001/api',
  timestamp: new Date().toISOString(),
});

export default clientLogger;
