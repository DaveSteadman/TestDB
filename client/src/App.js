import React, { useState, useEffect } from 'react';
import './styles/App.css';
import Login from './components/Login';
import Navbar from './components/Navbar';
import Dashboard from './components/Dashboard';
import Requirements from './components/Requirements';
import TestCases from './components/TestCases';
import Mappings from './components/Mappings';
import { authService } from './services/api';
import clientLogger from './services/clientLogger';
import api from './services/api';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState(null);
  const [currentPage, setCurrentPage] = useState('dashboard');
  const [connectionStatus, setConnectionStatus] = useState('checking'); // 'checking', 'connected', 'disconnected'
  const skipAuth = process.env.REACT_APP_SKIP_AUTH !== 'false';

  useEffect(() => {
    clientLogger.log('APP_INIT', 'Application starting', {
      skipAuth,
      apiUrl: process.env.REACT_APP_API_URL || 'http://localhost:5001/api',
    });

    // Check server connection
    checkServerConnection();

    if (skipAuth) {
      clientLogger.log('APP_AUTH_SKIP', 'Authentication skipped via REACT_APP_SKIP_AUTH');
      setIsAuthenticated(true);
      setUser({ username: 'demo' });
      return;
    }
    if (authService.isAuthenticated()) {
      clientLogger.log('APP_AUTH_RESTORED', 'Restored authentication from storage');
      setIsAuthenticated(true);
      setUser(authService.getCurrentUser());
    } else {
      clientLogger.log('APP_AUTH_REQUIRED', 'No authentication found, login required');
    }
  }, [skipAuth]);

  const checkServerConnection = async () => {
    clientLogger.logConnectionAttempt(api.defaults.baseURL);
    try {
      const response = await api.get('/health', { timeout: 5000 });
      if (response.status === 200) {
        setConnectionStatus('connected');
        clientLogger.logConnectionSuccess(api.defaults.baseURL);
      }
    } catch (error) {
      setConnectionStatus('disconnected');
      clientLogger.logConnectionFailure(api.defaults.baseURL, error);

      // Provide helpful error messages
      if (error.code === 'ECONNABORTED') {
        clientLogger.log('CONNECTION_ERROR', 'Server connection timeout - is the server running?');
      } else if (error.message === 'Network Error') {
        clientLogger.log('CONNECTION_ERROR', 'Network error - check if server is running and CORS is configured');
      }
    }
  };

  const handleLogin = () => {
    setIsAuthenticated(true);
    setUser(authService.getCurrentUser());
    clientLogger.log('APP_LOGIN_SUCCESS', 'User logged in successfully');
    // Recheck connection after login
    checkServerConnection();
  };

  const renderPage = () => {
    switch (currentPage) {
      case 'dashboard':
        return <Dashboard />;
      case 'requirements':
        return <Requirements />;
      case 'test-cases':
        return <TestCases />;
      case 'mappings':
        return <Mappings />;
      default:
        return <Dashboard />;
    }
  };

  // Show connection status banner
  const ConnectionBanner = () => {
    if (connectionStatus === 'connected') {
      return null; // Don't show banner when connected
    }

    return (
      <div style={{
        backgroundColor: connectionStatus === 'checking' ? '#fff3cd' : '#f8d7da',
        color: connectionStatus === 'checking' ? '#856404' : '#721c24',
        padding: '10px',
        textAlign: 'center',
        borderBottom: '1px solid ' + (connectionStatus === 'checking' ? '#ffc107' : '#dc3545'),
      }}>
        {connectionStatus === 'checking' && '🔄 Checking server connection...'}
        {connectionStatus === 'disconnected' && (
          <>
            ⚠️ Cannot connect to server. Please ensure the server is running at {api.defaults.baseURL}
            {' '}
            <button
              onClick={checkServerConnection}
              style={{
                marginLeft: '10px',
                padding: '5px 10px',
                cursor: 'pointer',
                border: '1px solid #721c24',
                backgroundColor: 'white',
                borderRadius: '3px',
              }}
            >
              Retry
            </button>
          </>
        )}
      </div>
    );
  };

  if (!isAuthenticated) {
    return (
      <>
        <ConnectionBanner />
        <Login onLogin={handleLogin} />
      </>
    );
  }

  return (
    <div className="App">
      <ConnectionBanner />
      <Navbar user={user} currentPage={currentPage} onNavigate={setCurrentPage} />
      {renderPage()}
    </div>
  );
}

export default App;
