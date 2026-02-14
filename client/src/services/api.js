import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:3001/api';

const api = axios.create({
  baseURL: API_BASE_URL,
});

// Add token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle unauthorized responses
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authService = {
  login: async (username, password) => {
    const response = await api.post('/auth/login', { username, password });
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.user));
    }
    return response.data;
  },
  
  register: async (username, password, email) => {
    const response = await api.post('/auth/register', { username, password, email });
    return response.data;
  },
  
  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },
  
  getCurrentUser: () => {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },
  
  isAuthenticated: () => {
    return !!localStorage.getItem('token');
  },
};

export const requirementsService = {
  getAll: async () => {
    const response = await api.get('/requirements');
    return response.data;
  },
  
  getById: async (id) => {
    const response = await api.get(`/requirements/${id}`);
    return response.data;
  },
  
  create: async (data) => {
    const response = await api.post('/requirements', data);
    return response.data;
  },
  
  update: async (id, data) => {
    const response = await api.put(`/requirements/${id}`, data);
    return response.data;
  },
  
  delete: async (id) => {
    const response = await api.delete(`/requirements/${id}`);
    return response.data;
  },
  
  getTestCases: async (id) => {
    const response = await api.get(`/requirements/${id}/test-cases`);
    return response.data;
  },
};

export const testCasesService = {
  getAll: async () => {
    const response = await api.get('/test-cases');
    return response.data;
  },
  
  getById: async (id) => {
    const response = await api.get(`/test-cases/${id}`);
    return response.data;
  },
  
  create: async (data) => {
    const response = await api.post('/test-cases', data);
    return response.data;
  },
  
  update: async (id, data) => {
    const response = await api.put(`/test-cases/${id}`, data);
    return response.data;
  },
  
  delete: async (id) => {
    const response = await api.delete(`/test-cases/${id}`);
    return response.data;
  },
  
  getRequirements: async (id) => {
    const response = await api.get(`/test-cases/${id}/requirements`);
    return response.data;
  },
};

export const mappingsService = {
  getAll: async () => {
    const response = await api.get('/mappings');
    return response.data;
  },
  
  create: async (requirementId, testCaseId) => {
    const response = await api.post('/mappings', {
      requirement_id: requirementId,
      test_case_id: testCaseId,
    });
    return response.data;
  },
  
  delete: async (id) => {
    const response = await api.delete(`/mappings/${id}`);
    return response.data;
  },
};

export default api;
