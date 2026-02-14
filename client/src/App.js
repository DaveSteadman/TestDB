import React, { useState, useEffect } from 'react';
import './styles/App.css';
import Login from './components/Login';
import Navbar from './components/Navbar';
import Dashboard from './components/Dashboard';
import Requirements from './components/Requirements';
import TestCases from './components/TestCases';
import Mappings from './components/Mappings';
import { authService } from './services/api';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState(null);
  const [currentPage, setCurrentPage] = useState('dashboard');
  const skipAuth = process.env.REACT_APP_SKIP_AUTH !== 'false';

  useEffect(() => {
    if (skipAuth) {
      setIsAuthenticated(true);
      setUser({ username: 'demo' });
      return;
    }
    if (authService.isAuthenticated()) {
      setIsAuthenticated(true);
      setUser(authService.getCurrentUser());
    }
  }, [skipAuth]);

  const handleLogin = () => {
    setIsAuthenticated(true);
    setUser(authService.getCurrentUser());
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

  if (!isAuthenticated) {
    return <Login onLogin={handleLogin} />;
  }

  return (
    <div className="App">
      <Navbar user={user} currentPage={currentPage} onNavigate={setCurrentPage} />
      {renderPage()}
    </div>
  );
}

export default App;
