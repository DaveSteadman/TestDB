const sqlite3 = require('sqlite3').verbose();
const bcrypt = require('bcryptjs');
const path = require('path');

const dbPath = path.join(__dirname, 'testdb.db');
const db = new sqlite3.Database(dbPath);

db.serialize(() => {
  // Create users table
  db.run(`
    CREATE TABLE IF NOT EXISTS users (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      username TEXT UNIQUE NOT NULL,
      password TEXT NOT NULL,
      email TEXT UNIQUE NOT NULL,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP
    )
  `);

  // Create requirements table
  db.run(`
    CREATE TABLE IF NOT EXISTS requirements (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      title TEXT NOT NULL,
      description TEXT,
      priority TEXT CHECK(priority IN ('Low', 'Medium', 'High', 'Critical')) DEFAULT 'Medium',
      status TEXT CHECK(status IN ('Draft', 'Active', 'Completed', 'Deprecated')) DEFAULT 'Draft',
      created_by INTEGER,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (created_by) REFERENCES users(id)
    )
  `);

  // Create test_cases table
  db.run(`
    CREATE TABLE IF NOT EXISTS test_cases (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      title TEXT NOT NULL,
      description TEXT,
      steps TEXT,
      expected_result TEXT,
      status TEXT CHECK(status IN ('Draft', 'Active', 'Passed', 'Failed', 'Blocked')) DEFAULT 'Draft',
      created_by INTEGER,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (created_by) REFERENCES users(id)
    )
  `);

  // Create requirement_test_mapping table
  db.run(`
    CREATE TABLE IF NOT EXISTS requirement_test_mapping (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      requirement_id INTEGER NOT NULL,
      test_case_id INTEGER NOT NULL,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (requirement_id) REFERENCES requirements(id) ON DELETE CASCADE,
      FOREIGN KEY (test_case_id) REFERENCES test_cases(id) ON DELETE CASCADE,
      UNIQUE(requirement_id, test_case_id)
    )
  `);

  // Create a default admin user
  const password = bcrypt.hashSync('admin123', 10);
  db.run(
    'INSERT OR IGNORE INTO users (username, password, email) VALUES (?, ?, ?)',
    ['admin', password, 'admin@testdb.com'],
    (err) => {
      if (err) {
        console.error('Error creating admin user:', err);
      } else {
        console.log('Admin user created (username: admin, password: admin123)');
      }
    }
  );

  console.log('Database initialized successfully!');
  console.log('Tables created: users, requirements, test_cases, requirement_test_mapping');
});

db.close();
