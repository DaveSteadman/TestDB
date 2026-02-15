import axios from 'axios';
import clientLogger from './clientLogger';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5001/api';

clientLogger.log('API_INIT', `Initializing API with base URL: ${API_BASE_URL}`);

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000, // 10 second timeout
});

// Add token to requests and log
api.interceptors.request.use((config) => {
  // Store request start time for duration tracking
  config.metadata = { startTime: new Date().getTime() };

  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  // Log the request
  clientLogger.logApiRequest(config.method.toUpperCase(), config.url);

  return config;
}, (error) => {
  clientLogger.log('API_REQUEST_ERROR', 'Request interceptor error', { error: error.message });
  return Promise.reject(error);
});

const skipAuth = process.env.REACT_APP_SKIP_AUTH !== 'false';

// Handle responses and errors with logging
api.interceptors.response.use(
  (response) => {
    // Calculate request duration
    const duration = new Date().getTime() - response.config.metadata.startTime;

    // Log successful response
    clientLogger.logApiResponse(
      response.config.method.toUpperCase(),
      response.config.url,
      response.status,
      duration
    );

    return response;
  },
  (error) => {
    // Calculate duration if available
    let duration = 'N/A';
    if (error.config?.metadata?.startTime) {
      duration = new Date().getTime() - error.config.metadata.startTime;
    }

    // Log the error
    clientLogger.logApiError(
      error.config?.method?.toUpperCase() || 'UNKNOWN',
      error.config?.url || 'unknown',
      error
    );

    // Handle specific error cases
    if (error.code === 'ECONNABORTED') {
      clientLogger.log('API_TIMEOUT', `Request timeout after ${duration}ms: ${error.config?.url}`);
    } else if (error.message === 'Network Error') {
      clientLogger.logConnectionFailure(API_BASE_URL, error);
    } else if (error.response?.status === 401) {
      clientLogger.log('AUTH_EXPIRED', 'Authentication token expired or invalid');
      localStorage.removeItem('token');
      if (!skipAuth) {
        window.location.href = '/login';
      }
    }

    return Promise.reject(error);
  }
);

export const authService = {
  login: async (username, password) => {
    clientLogger.logAuthAttempt(username);
    try {
      const response = await api.post('/auth/login', { username, password });
      if (response.data.token) {
        localStorage.setItem('token', response.data.token);
        localStorage.setItem('user', JSON.stringify(response.data.user));
        clientLogger.logAuthSuccess(username);
      }
      return response.data;
    } catch (error) {
      const errorMsg = error.response?.data?.error || error.message || 'Unknown error';
      clientLogger.logAuthFailure(username, errorMsg);
      throw error;
    }
  },

  register: async (username, password, email) => {
    clientLogger.log('AUTH_REGISTER', `Registration attempt for user: ${username}`);
    try {
      const response = await api.post('/auth/register', { username, password, email });
      clientLogger.log('AUTH_REGISTER_SUCCESS', `Registration successful for user: ${username}`);
      return response.data;
    } catch (error) {
      const errorMsg = error.response?.data?.error || error.message || 'Unknown error';
      clientLogger.log('AUTH_REGISTER_FAILURE', `Registration failed for user: ${username}: ${errorMsg}`);
      throw error;
    }
  },

  logout: () => {
    const username = authService.getCurrentUser()?.username || 'unknown';
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    clientLogger.log('AUTH_LOGOUT', `User logged out: ${username}`);
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
