import React, { useState, useEffect } from 'react';
import { testCasesService } from '../services/api';

function TestCases() {
  const [testCases, setTestCases] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    steps: '',
    expected_result: '',
    status: 'Draft',
  });

  useEffect(() => {
    loadTestCases();
  }, []);

  const loadTestCases = async () => {
    try {
      const data = await testCasesService.getAll();
      setTestCases(data);
    } catch (error) {
      console.error('Error loading test cases:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingId) {
        await testCasesService.update(editingId, formData);
      } else {
        await testCasesService.create(formData);
      }
      setShowModal(false);
      resetForm();
      loadTestCases();
    } catch (error) {
      console.error('Error saving test case:', error);
      alert('Error saving test case');
    }
  };

  const handleEdit = (testCase) => {
    setEditingId(testCase.id);
    setFormData({
      title: testCase.title,
      description: testCase.description || '',
      steps: testCase.steps || '',
      expected_result: testCase.expected_result || '',
      status: testCase.status,
    });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this test case?')) {
      try {
        await testCasesService.delete(id);
        loadTestCases();
      } catch (error) {
        console.error('Error deleting test case:', error);
        alert('Error deleting test case');
      }
    }
  };

  const resetForm = () => {
    setFormData({
      title: '',
      description: '',
      steps: '',
      expected_result: '',
      status: 'Draft',
    });
    setEditingId(null);
  };

  if (loading) {
    return <div className="loading">Loading test cases...</div>;
  }

  return (
    <div className="container">
      <div className="dashboard">
        <div className="list-header">
          <h2>🧪 Test Cases</h2>
          <button
            className="btn-add"
            onClick={() => {
              resetForm();
              setShowModal(true);
            }}
          >
            + Add Test Case
          </button>
        </div>

        {testCases.length === 0 ? (
          <div className="empty-state">
            <div className="empty-state-icon">🧪</div>
            <h3>No test cases yet</h3>
            <p>Click "Add Test Case" to create your first test case</p>
          </div>
        ) : (
          <div className="items-list">
            {testCases.map((test) => (
              <div key={test.id} className="item-card">
                <h3>{test.title}</h3>
                <p>{test.description}</p>
                {test.steps && (
                  <div style={{ marginTop: '10px', fontSize: '14px', color: '#555' }}>
                    <strong>Steps:</strong> {test.steps}
                  </div>
                )}
                <div className="item-meta" style={{ marginTop: '10px' }}>
                  <span className={`badge badge-status-${test.status.toLowerCase()}`}>
                    {test.status}
                  </span>
                  <span className="badge">By: {test.creator || 'Unknown'}</span>
                </div>
                <div className="item-actions">
                  <button className="btn-small btn-edit" onClick={() => handleEdit(test)}>
                    Edit
                  </button>
                  <button className="btn-small btn-delete" onClick={() => handleDelete(test.id)}>
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}

        {showModal && (
          <div className="modal-overlay" onClick={() => setShowModal(false)}>
            <div className="modal" onClick={(e) => e.stopPropagation()}>
              <h2>{editingId ? 'Edit Test Case' : 'New Test Case'}</h2>
              <form onSubmit={handleSubmit}>
                <div className="form-group">
                  <label>Title *</label>
                  <input
                    type="text"
                    value={formData.title}
                    onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                    required
                  />
                </div>
                <div className="form-group">
                  <label>Description</label>
                  <textarea
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  />
                </div>
                <div className="form-group">
                  <label>Test Steps</label>
                  <textarea
                    value={formData.steps}
                    onChange={(e) => setFormData({ ...formData, steps: e.target.value })}
                    placeholder="1. Step one&#10;2. Step two&#10;3. Step three..."
                  />
                </div>
                <div className="form-group">
                  <label>Expected Result</label>
                  <textarea
                    value={formData.expected_result}
                    onChange={(e) => setFormData({ ...formData, expected_result: e.target.value })}
                  />
                </div>
                <div className="form-group">
                  <label>Status</label>
                  <select
                    value={formData.status}
                    onChange={(e) => setFormData({ ...formData, status: e.target.value })}
                  >
                    <option value="Draft">Draft</option>
                    <option value="Active">Active</option>
                    <option value="Passed">Passed</option>
                    <option value="Failed">Failed</option>
                    <option value="Blocked">Blocked</option>
                  </select>
                </div>
                <div className="modal-actions">
                  <button type="submit" className="btn btn-primary">
                    {editingId ? 'Update' : 'Create'}
                  </button>
                  <button
                    type="button"
                    className="btn btn-secondary"
                    onClick={() => {
                      setShowModal(false);
                      resetForm();
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

export default TestCases;
