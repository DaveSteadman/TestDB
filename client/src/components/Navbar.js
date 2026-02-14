import React from 'react';
import { authService } from '../services/api';

function Navbar({ user, currentPage, onNavigate }) {
  const handleLogout = () => {
    authService.logout();
    window.location.reload();
  };

  return (
    <nav className="navbar">
      <div className="navbar-brand">TestDB</div>
      <div className="navbar-menu">
        <a
          href="#dashboard"
          className={`navbar-link ${currentPage === 'dashboard' ? 'active' : ''}`}
          onClick={(e) => {
            e.preventDefault();
            onNavigate('dashboard');
          }}
        >
          Dashboard
        </a>
        <a
          href="#requirements"
          className={`navbar-link ${currentPage === 'requirements' ? 'active' : ''}`}
          onClick={(e) => {
            e.preventDefault();
            onNavigate('requirements');
          }}
        >
          Requirements
        </a>
        <a
          href="#test-cases"
          className={`navbar-link ${currentPage === 'test-cases' ? 'active' : ''}`}
          onClick={(e) => {
            e.preventDefault();
            onNavigate('test-cases');
          }}
        >
          Test Cases
        </a>
        <a
          href="#mappings"
          className={`navbar-link ${currentPage === 'mappings' ? 'active' : ''}`}
          onClick={(e) => {
            e.preventDefault();
            onNavigate('mappings');
          }}
        >
          Mappings
        </a>
        <span className="navbar-user">👤 {user?.username}</span>
        <button className="btn-logout" onClick={handleLogout}>
          Logout
        </button>
      </div>
    </nav>
  );
}

export default Navbar;
