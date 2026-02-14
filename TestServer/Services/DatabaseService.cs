using Microsoft.Data.Sqlite;
using TestServer.Models;

namespace TestServer.Services;

public class DatabaseService : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public DatabaseService(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        InitializeDatabaseSync();
    }

    private void InitializeDatabaseSync()
    {
        _semaphore.Wait();
        try
        {
            using var command = _connection.CreateCommand();
            
            // Create Users table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                )";
            command.ExecuteNonQuery();

            // Create Requirements table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Requirements (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Text TEXT NOT NULL
                )";
            command.ExecuteNonQuery();

            // Create TestCases table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS TestCases (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Text TEXT NOT NULL
                )";
            command.ExecuteNonQuery();

            // Create RequirementTestCases mapping table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS RequirementTestCases (
                    RequirementId INTEGER NOT NULL,
                    TestCaseId INTEGER NOT NULL,
                    PRIMARY KEY (RequirementId, TestCaseId),
                    FOREIGN KEY (RequirementId) REFERENCES Requirements(Id) ON DELETE CASCADE,
                    FOREIGN KEY (TestCaseId) REFERENCES TestCases(Id) ON DELETE CASCADE
                )";
            command.ExecuteNonQuery();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // User operations
    public async Task<List<User>> GetUsersAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var users = new List<User>();
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT Id, Name FROM Users";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
            return users;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT Id, Name FROM Users WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
            }
            return null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<User> CreateUserAsync(string name)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO Users (Name) VALUES (@Name); SELECT last_insert_rowid()";
            command.Parameters.AddWithValue("@Name", name);
            
            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            return new User { Id = id, Name = name };
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> UpdateUserAsync(int id, string name)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "UPDATE Users SET Name = @Name WHERE Id = @Id";
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Id", id);
            
            return await command.ExecuteNonQueryAsync() > 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            
            return await command.ExecuteNonQueryAsync() > 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // Requirement operations
    public async Task<List<Requirement>> GetRequirementsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var requirements = new List<Requirement>();
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT Id, Text FROM Requirements";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                requirements.Add(new Requirement
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1)
                });
            }
            return requirements;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Requirement?> GetRequirementByIdAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT Id, Text FROM Requirements WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Requirement
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1)
                };
            }
            return null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Requirement> CreateRequirementAsync(string text)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO Requirements (Text) VALUES (@Text); SELECT last_insert_rowid()";
            command.Parameters.AddWithValue("@Text", text);
            
            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            return new Requirement { Id = id, Text = text };
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> UpdateRequirementAsync(int id, string text)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "UPDATE Requirements SET Text = @Text WHERE Id = @Id";
            command.Parameters.AddWithValue("@Text", text);
            command.Parameters.AddWithValue("@Id", id);
            
            return await command.ExecuteNonQueryAsync() > 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> DeleteRequirementAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM Requirements WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            
            return await command.ExecuteNonQueryAsync() > 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // TestCase operations
    public async Task<List<TestCase>> GetTestCasesAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var testCases = new List<TestCase>();
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT Id, Text FROM TestCases";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                testCases.Add(new TestCase
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1)
                });
            }
            return testCases;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<TestCase?> GetTestCaseByIdAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT Id, Text FROM TestCases WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new TestCase
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1)
                };
            }
            return null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<TestCase> CreateTestCaseAsync(string text)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO TestCases (Text) VALUES (@Text); SELECT last_insert_rowid()";
            command.Parameters.AddWithValue("@Text", text);
            
            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            return new TestCase { Id = id, Text = text };
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> UpdateTestCaseAsync(int id, string text)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "UPDATE TestCases SET Text = @Text WHERE Id = @Id";
            command.Parameters.AddWithValue("@Text", text);
            command.Parameters.AddWithValue("@Id", id);
            
            return await command.ExecuteNonQueryAsync() > 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> DeleteTestCaseAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM TestCases WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            
            return await command.ExecuteNonQueryAsync() > 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // RequirementTestCase mapping operations
    public async Task<List<RequirementTestCase>> GetRequirementTestCasesAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var mappings = new List<RequirementTestCase>();
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT RequirementId, TestCaseId FROM RequirementTestCases";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                mappings.Add(new RequirementTestCase
                {
                    RequirementId = reader.GetInt32(0),
                    TestCaseId = reader.GetInt32(1)
                });
            }
            return mappings;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<TestCase>> GetTestCasesForRequirementAsync(int requirementId)
    {
        await _semaphore.WaitAsync();
        try
        {
            var testCases = new List<TestCase>();
            using var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT tc.Id, tc.Text 
                FROM TestCases tc
                INNER JOIN RequirementTestCases rtc ON tc.Id = rtc.TestCaseId
                WHERE rtc.RequirementId = @RequirementId";
            command.Parameters.AddWithValue("@RequirementId", requirementId);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                testCases.Add(new TestCase
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1)
                });
            }
            return testCases;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<Requirement>> GetRequirementsForTestCaseAsync(int testCaseId)
    {
        await _semaphore.WaitAsync();
        try
        {
            var requirements = new List<Requirement>();
            using var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT r.Id, r.Text 
                FROM Requirements r
                INNER JOIN RequirementTestCases rtc ON r.Id = rtc.RequirementId
                WHERE rtc.TestCaseId = @TestCaseId";
            command.Parameters.AddWithValue("@TestCaseId", testCaseId);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                requirements.Add(new Requirement
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1)
                });
            }
            return requirements;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> AddRequirementTestCaseMappingAsync(int requirementId, int testCaseId)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = @"
                INSERT OR IGNORE INTO RequirementTestCases (RequirementId, TestCaseId) 
                VALUES (@RequirementId, @TestCaseId)";
            command.Parameters.AddWithValue("@RequirementId", requirementId);
            command.Parameters.AddWithValue("@TestCaseId", testCaseId);
            
            return await command.ExecuteNonQueryAsync() > 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> RemoveRequirementTestCaseMappingAsync(int requirementId, int testCaseId)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM RequirementTestCases 
                WHERE RequirementId = @RequirementId AND TestCaseId = @TestCaseId";
            command.Parameters.AddWithValue("@RequirementId", requirementId);
            command.Parameters.AddWithValue("@TestCaseId", testCaseId);
            
            return await command.ExecuteNonQueryAsync() > 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
        _connection.Dispose();
    }
}
