import React, { useState, useEffect } from 'react';
import { requirementsService, testCasesService } from '../services/api';

function Dashboard() {
  const [stats, setStats] = useState({
    requirements: 0,
    testCases: 0,
    activeRequirements: 0,
    passedTests: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    try {
      const [requirements, testCases] = await Promise.all([
        requirementsService.getAll(),
        testCasesService.getAll(),
      ]);

      setStats({
        requirements: requirements.length,
        testCases: testCases.length,
        activeRequirements: requirements.filter((r) => r.status === 'Active').length,
        passedTests: testCases.filter((t) => t.status === 'Passed').length,
      });
    } catch (error) {
      console.error('Error loading stats:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading">Loading dashboard...</div>;
  }

  return (
    <div className="container">
      <div className="dashboard">
        <h2>📊 Dashboard</h2>
        <div className="stats-grid">
          <div className="stat-card">
            <h3>{stats.requirements}</h3>
            <p>Total Requirements</p>
          </div>
          <div className="stat-card">
            <h3>{stats.testCases}</h3>
            <p>Total Test Cases</p>
          </div>
          <div className="stat-card">
            <h3>{stats.activeRequirements}</h3>
            <p>Active Requirements</p>
          </div>
          <div className="stat-card">
            <h3>{stats.passedTests}</h3>
            <p>Passed Tests</p>
          </div>
        </div>
        <div style={{ marginTop: '30px', color: '#666' }}>
          <h3 style={{ marginBottom: '15px' }}>Welcome to TestDB!</h3>
          <p style={{ lineHeight: '1.8' }}>
            This is a comprehensive requirements and test case management system. Use the navigation
            above to:
          </p>
          <ul style={{ marginTop: '15px', marginLeft: '20px', lineHeight: '1.8' }}>
            <li>Create and manage software requirements</li>
            <li>Create and manage test cases</li>
            <li>Map requirements to their corresponding test cases</li>
            <li>Track the status of requirements and tests</li>
          </ul>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
