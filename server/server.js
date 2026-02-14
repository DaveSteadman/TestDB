const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');
const cors = require('cors');
const bodyParser = require('body-parser');
const path = require('path');

const app = express();
const PORT = process.env.PORT || 3001;
const JWT_SECRET = process.env.JWT_SECRET || 'your-secret-key-change-in-production';

// Warning for production
if (!process.env.JWT_SECRET && process.env.NODE_ENV === 'production') {
  console.warn('WARNING: Using default JWT secret in production is insecure! Set JWT_SECRET environment variable.');
}

// Middleware
const corsOptions = {
  origin: process.env.CORS_ORIGIN || 'http://localhost:3000',
  credentials: true
};
app.use(cors(corsOptions));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// Database connection
const dbPath = path.join(__dirname, 'testdb.db');
const db = new sqlite3.Database(dbPath, (err) => {
  if (err) {
    console.error('Error connecting to database:', err);
  } else {
    console.log('Connected to SQLite database');
  }
});

// Authentication middleware
const authenticateToken = (req, res, next) => {
  const authHeader = req.headers['authorization'];
  const token = authHeader && authHeader.split(' ')[1];

  if (!token) {
    return res.status(401).json({ error: 'Access token required' });
  }

  jwt.verify(token, JWT_SECRET, (err, user) => {
    if (err) {
      return res.status(403).json({ error: 'Invalid or expired token' });
    }
    req.user = user;
    next();
  });
};

// Auth Routes
app.post('/api/auth/login', (req, res) => {
  const { username, password } = req.body;

  db.get('SELECT * FROM users WHERE username = ?', [username], (err, user) => {
    if (err) {
      return res.status(500).json({ error: 'Database error' });
    }
    if (!user) {
      return res.status(401).json({ error: 'Invalid credentials' });
    }

    bcrypt.compare(password, user.password, (err, result) => {
      if (result) {
        const token = jwt.sign(
          { id: user.id, username: user.username, email: user.email },
          JWT_SECRET,
          { expiresIn: '24h' }
        );
        res.json({ token, user: { id: user.id, username: user.username, email: user.email } });
      } else {
        res.status(401).json({ error: 'Invalid credentials' });
      }
    });
  });
});

app.post('/api/auth/register', (req, res) => {
  const { username, password, email } = req.body;

  if (!username || !password || !email) {
    return res.status(400).json({ error: 'All fields are required' });
  }

  const hashedPassword = bcrypt.hashSync(password, 10);

  db.run(
    'INSERT INTO users (username, password, email) VALUES (?, ?, ?)',
    [username, hashedPassword, email],
    function (err) {
      if (err) {
        if (err.message.includes('UNIQUE')) {
          return res.status(400).json({ error: 'Username or email already exists' });
        }
        return res.status(500).json({ error: 'Database error' });
      }
      res.status(201).json({ message: 'User registered successfully', userId: this.lastID });
    }
  );
});

// Requirements Routes
app.get('/api/requirements', authenticateToken, (req, res) => {
  db.all(
    'SELECT r.*, u.username as creator FROM requirements r LEFT JOIN users u ON r.created_by = u.id ORDER BY r.created_at DESC',
    [],
    (err, rows) => {
      if (err) {
        return res.status(500).json({ error: 'Database error' });
      }
      res.json(rows);
    }
  );
});

app.get('/api/requirements/:id', authenticateToken, (req, res) => {
  db.get('SELECT * FROM requirements WHERE id = ?', [req.params.id], (err, row) => {
    if (err) {
      return res.status(500).json({ error: 'Database error' });
    }
    if (!row) {
      return res.status(404).json({ error: 'Requirement not found' });
    }
    res.json(row);
  });
});

app.post('/api/requirements', authenticateToken, (req, res) => {
  const { title, description, priority, status } = req.body;

  db.run(
    'INSERT INTO requirements (title, description, priority, status, created_by) VALUES (?, ?, ?, ?, ?)',
    [title, description, priority || 'Medium', status || 'Draft', req.user.id],
    function (err) {
      if (err) {
        return res.status(500).json({ error: 'Database error' });
      }
      res.status(201).json({ id: this.lastID, message: 'Requirement created successfully' });
    }
  );
});

app.put('/api/requirements/:id', authenticateToken, (req, res) => {
  const { title, description, priority, status } = req.body;

  db.run(
    'UPDATE requirements SET title = ?, description = ?, priority = ?, status = ?, updated_at = CURRENT_TIMESTAMP WHERE id = ?',
    [title, description, priority, status, req.params.id],
    function (err) {
      if (err) {
        return res.status(500).json({ error: 'Database error' });
      }
      if (this.changes === 0) {
        return res.status(404).json({ error: 'Requirement not found' });
      }
      res.json({ message: 'Requirement updated successfully' });
    }
  );
});

app.delete('/api/requirements/:id', authenticateToken, (req, res) => {
  db.run('DELETE FROM requirements WHERE id = ?', [req.params.id], function (err) {
    if (err) {
      return res.status(500).json({ error: 'Database error' });
    }
    if (this.changes === 0) {
      return res.status(404).json({ error: 'Requirement not found' });
    }
    res.json({ message: 'Requirement deleted successfully' });
  });
});

// Test Cases Routes
app.get('/api/test-cases', authenticateToken, (req, res) => {
  db.all(
    'SELECT t.*, u.username as creator FROM test_cases t LEFT JOIN users u ON t.created_by = u.id ORDER BY t.created_at DESC',
    [],
    (err, rows) => {
      if (err) {
        return res.status(500).json({ error: 'Database error' });
      }
      res.json(rows);
    }
  );
});

app.get('/api/test-cases/:id', authenticateToken, (req, res) => {
  db.get('SELECT * FROM test_cases WHERE id = ?', [req.params.id], (err, row) => {
    if (err) {
      return res.status(500).json({ error: 'Database error' });
    }
    if (!row) {
      return res.status(404).json({ error: 'Test case not found' });
    }
    res.json(row);
  });
});

app.post('/api/test-cases', authenticateToken, (req, res) => {
  const { title, description, steps, expected_result, status } = req.body;

  db.run(
    'INSERT INTO test_cases (title, description, steps, expected_result, status, created_by) VALUES (?, ?, ?, ?, ?, ?)',
    [title, description, steps, expected_result, status || 'Draft', req.user.id],
    function (err) {
      if (err) {
        return res.status(500).json({ error: 'Database error' });
      }
      res.status(201).json({ id: this.lastID, message: 'Test case created successfully' });
    }
  );
});

app.put('/api/test-cases/:id', authenticateToken, (req, res) => {
  const { title, description, steps, expected_result, status } = req.body;

  db.run(
    'UPDATE test_cases SET title = ?, description = ?, steps = ?, expected_result = ?, status = ?, updated_at = CURRENT_TIMESTAMP WHERE id = ?',
    [title, description, steps, expected_result, status, req.params.id],
    function (err) {
      if (err) {
        return res.status(500).json({ error: 'Database error' });
      }
      if (this.changes === 0) {
        return res.status(404).json({ error: 'Test case not found' });
      }
      res.json({ message: 'Test case updated successfully' });
    }
  );
});

app.delete('/api/test-cases/:id', authenticateToken, (req, res) => {
  db.run('DELETE FROM test_cases WHERE id = ?', [req.params.id], function (err) {
    if (err) {
      return res.status(500).json({ error: 'Database error' });
    }
    if (this.changes === 0) {
      return res.status(404).json({ error: 'Test case not found' });
    }
    res.json({ message: 'Test case deleted successfully' });
  });
});

// Requirement-Test Mapping Routes
app.get('/api/mappings', authenticateToken, (req, res) => {
  db.all(
    `SELECT m.*, r.title as requirement_title, t.title as test_case_title 
     FROM requirement_test_mapping m 
     LEFT JOIN requirements r ON m.requirement_id = r.id 
     LEFT JOIN test_cases t ON m.test_case_id = t.id`,
    [],
    (err, rows) => {
      if (err) {
        return res.status(500).json({ error: 'Database error' });
      }
      res.json(rows);
    }
  );
});

app.get('/api/requirements/:id/test-cases', authenticateToken, (req, res) => {
  db.all(
    `SELECT t.* FROM test_cases t 
     INNER JOIN requirement_test_mapping m ON t.id = m.test_case_id 
     WHERE m.requirement_id = ?`,
    [req.params.id],
    (err, rows) => {
      if (err) {
        return res.status(500).json({ error: 'Database error' });
      }
      res.json(rows);
    }
  );
});

app.get('/api/test-cases/:id/requirements', authenticateToken, (req, res) => {
  db.all(
    `SELECT r.* FROM requirements r 
     INNER JOIN requirement_test_mapping m ON r.id = m.requirement_id 
     WHERE m.test_case_id = ?`,
    [req.params.id],
    (err, rows) => {
      if (err) {
        return res.status(500).json({ error: 'Database error' });
      }
      res.json(rows);
    }
  );
});

app.post('/api/mappings', authenticateToken, (req, res) => {
  const { requirement_id, test_case_id } = req.body;

  db.run(
    'INSERT INTO requirement_test_mapping (requirement_id, test_case_id) VALUES (?, ?)',
    [requirement_id, test_case_id],
    function (err) {
      if (err) {
        if (err.message.includes('UNIQUE')) {
          return res.status(400).json({ error: 'Mapping already exists' });
        }
        return res.status(500).json({ error: 'Database error' });
      }
      res.status(201).json({ id: this.lastID, message: 'Mapping created successfully' });
    }
  );
});

app.delete('/api/mappings/:id', authenticateToken, (req, res) => {
  db.run('DELETE FROM requirement_test_mapping WHERE id = ?', [req.params.id], function (err) {
    if (err) {
      return res.status(500).json({ error: 'Database error' });
    }
    if (this.changes === 0) {
      return res.status(404).json({ error: 'Mapping not found' });
    }
    res.json({ message: 'Mapping deleted successfully' });
  });
});

// Health check
app.get('/api/health', (req, res) => {
  res.json({ status: 'ok', timestamp: new Date().toISOString() });
});

// Start server
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
  console.log(`API available at http://localhost:${PORT}/api`);
});
