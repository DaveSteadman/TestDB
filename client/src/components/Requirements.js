import React, { useState, useEffect } from 'react';
import { requirementsService } from '../services/api';

function Requirements() {
  const [requirements, setRequirements] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    priority: 'Medium',
    status: 'Draft',
  });

  useEffect(() => {
    loadRequirements();
  }, []);

  const loadRequirements = async () => {
    try {
      const data = await requirementsService.getAll();
      setRequirements(data);
    } catch (error) {
      console.error('Error loading requirements:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingId) {
        await requirementsService.update(editingId, formData);
      } else {
        await requirementsService.create(formData);
      }
      setShowModal(false);
      resetForm();
      loadRequirements();
    } catch (error) {
      console.error('Error saving requirement:', error);
      alert('Error saving requirement');
    }
  };

  const handleEdit = (requirement) => {
    setEditingId(requirement.id);
    setFormData({
      title: requirement.title,
      description: requirement.description || '',
      priority: requirement.priority,
      status: requirement.status,
    });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this requirement?')) {
      try {
        await requirementsService.delete(id);
        loadRequirements();
      } catch (error) {
        console.error('Error deleting requirement:', error);
        alert('Error deleting requirement');
      }
    }
  };

  const resetForm = () => {
    setFormData({
      title: '',
      description: '',
      priority: 'Medium',
      status: 'Draft',
    });
    setEditingId(null);
  };

  if (loading) {
    return <div className="loading">Loading requirements...</div>;
  }

  return (
    <div className="container">
      <div className="dashboard">
        <div className="list-header">
          <h2>📋 Requirements</h2>
          <button
            className="btn-add"
            onClick={() => {
              resetForm();
              setShowModal(true);
            }}
          >
            + Add Requirement
          </button>
        </div>

        {requirements.length === 0 ? (
          <div className="empty-state">
            <div className="empty-state-icon">📋</div>
            <h3>No requirements yet</h3>
            <p>Click "Add Requirement" to create your first requirement</p>
          </div>
        ) : (
          <div className="items-list">
            {requirements.map((req) => (
              <div key={req.id} className="item-card">
                <h3>{req.title}</h3>
                <p>{req.description}</p>
                <div className="item-meta">
                  <span className={`badge badge-priority-${req.priority.toLowerCase()}`}>
                    {req.priority}
                  </span>
                  <span className={`badge badge-status-${req.status.toLowerCase()}`}>
                    {req.status}
                  </span>
                  <span className="badge">By: {req.creator || 'Unknown'}</span>
                </div>
                <div className="item-actions">
                  <button className="btn-small btn-edit" onClick={() => handleEdit(req)}>
                    Edit
                  </button>
                  <button className="btn-small btn-delete" onClick={() => handleDelete(req.id)}>
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
              <h2>{editingId ? 'Edit Requirement' : 'New Requirement'}</h2>
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
                  <label>Priority</label>
                  <select
                    value={formData.priority}
                    onChange={(e) => setFormData({ ...formData, priority: e.target.value })}
                  >
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                    <option value="Critical">Critical</option>
                  </select>
                </div>
                <div className="form-group">
                  <label>Status</label>
                  <select
                    value={formData.status}
                    onChange={(e) => setFormData({ ...formData, status: e.target.value })}
                  >
                    <option value="Draft">Draft</option>
                    <option value="Active">Active</option>
                    <option value="Completed">Completed</option>
                    <option value="Deprecated">Deprecated</option>
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

export default Requirements;
