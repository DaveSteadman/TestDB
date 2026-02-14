import React, { useState, useEffect } from 'react';
import { mappingsService, requirementsService, testCasesService } from '../services/api';

function Mappings() {
  const [mappings, setMappings] = useState([]);
  const [requirements, setRequirements] = useState([]);
  const [testCases, setTestCases] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState({
    requirement_id: '',
    test_case_id: '',
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [mappingsData, requirementsData, testCasesData] = await Promise.all([
        mappingsService.getAll(),
        requirementsService.getAll(),
        testCasesService.getAll(),
      ]);
      setMappings(mappingsData);
      setRequirements(requirementsData);
      setTestCases(testCasesData);
    } catch (error) {
      console.error('Error loading data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await mappingsService.create(formData.requirement_id, formData.test_case_id);
      setShowModal(false);
      setFormData({ requirement_id: '', test_case_id: '' });
      loadData();
    } catch (error) {
      console.error('Error creating mapping:', error);
      alert(error.response?.data?.error || 'Error creating mapping');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this mapping?')) {
      try {
        await mappingsService.delete(id);
        loadData();
      } catch (error) {
        console.error('Error deleting mapping:', error);
        alert('Error deleting mapping');
      }
    }
  };

  if (loading) {
    return <div className="loading">Loading mappings...</div>;
  }

  return (
    <div className="container">
      <div className="dashboard">
        <div className="list-header">
          <h2>🔗 Requirement-Test Mappings</h2>
          <button className="btn-add" onClick={() => setShowModal(true)}>
            + Add Mapping
          </button>
        </div>

        {mappings.length === 0 ? (
          <div className="empty-state">
            <div className="empty-state-icon">🔗</div>
            <h3>No mappings yet</h3>
            <p>Click "Add Mapping" to link requirements with test cases</p>
          </div>
        ) : (
          <div className="items-list">
            {mappings.map((mapping) => (
              <div key={mapping.id} className="item-card">
                <div style={{ display: 'flex', gap: '20px', alignItems: 'center' }}>
                  <div style={{ flex: 1 }}>
                    <div style={{ fontSize: '12px', color: '#999', marginBottom: '5px' }}>
                      REQUIREMENT
                    </div>
                    <h3 style={{ fontSize: '16px' }}>{mapping.requirement_title}</h3>
                  </div>
                  <div
                    style={{
                      fontSize: '24px',
                      color: '#667eea',
                      fontWeight: 'bold',
                    }}
                  >
                    →
                  </div>
                  <div style={{ flex: 1 }}>
                    <div style={{ fontSize: '12px', color: '#999', marginBottom: '5px' }}>
                      TEST CASE
                    </div>
                    <h3 style={{ fontSize: '16px' }}>{mapping.test_case_title}</h3>
                  </div>
                </div>
                <div className="item-actions">
                  <button className="btn-small btn-delete" onClick={() => handleDelete(mapping.id)}>
                    Delete Mapping
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}

        {showModal && (
          <div className="modal-overlay" onClick={() => setShowModal(false)}>
            <div className="modal" onClick={(e) => e.stopPropagation()}>
              <h2>New Mapping</h2>
              <form onSubmit={handleSubmit}>
                <div className="form-group">
                  <label>Requirement *</label>
                  <select
                    value={formData.requirement_id}
                    onChange={(e) =>
                      setFormData({ ...formData, requirement_id: e.target.value })
                    }
                    required
                  >
                    <option value="">Select a requirement...</option>
                    {requirements.map((req) => (
                      <option key={req.id} value={req.id}>
                        {req.title}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="form-group">
                  <label>Test Case *</label>
                  <select
                    value={formData.test_case_id}
                    onChange={(e) =>
                      setFormData({ ...formData, test_case_id: e.target.value })
                    }
                    required
                  >
                    <option value="">Select a test case...</option>
                    {testCases.map((test) => (
                      <option key={test.id} value={test.id}>
                        {test.title}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="modal-actions">
                  <button type="submit" className="btn btn-primary">
                    Create Mapping
                  </button>
                  <button
                    type="button"
                    className="btn btn-secondary"
                    onClick={() => {
                      setShowModal(false);
                      setFormData({ requirement_id: '', test_case_id: '' });
                    }}
                  >
                    Cancel
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default Mappings;
