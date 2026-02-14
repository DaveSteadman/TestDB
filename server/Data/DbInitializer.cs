using Microsoft.EntityFrameworkCore;
using TestDB.Server.Models;

namespace TestDB.Server.Data;

/// <summary>
/// Database initializer that creates the database and seeds initial data
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Initializes the database and seeds default admin user
    /// </summary>
    public static void Initialize(TestDbContext context, ILogger logger)
    {
        try
        {
            // Create database and apply migrations
            context.Database.EnsureCreated();
            logger.LogInformation("Database initialized successfully");

            // Check if admin user already exists
            var adminExists = context.Users.Any(u => u.Username == "admin");
            if (!adminExists)
            {
                var adminPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
                var adminUser = new User
                {
                    Username = "admin",
                    Password = adminPassword,
                    Email = "admin@testdb.com",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
                logger.LogInformation("Admin user created (username: admin, password: admin123)");
            }
            else
            {
                logger.LogInformation("Admin user already exists");
            }

            logger.LogInformation("Tables: users, requirements, test_cases, requirement_test_mapping");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error initializing database");
            throw;
        }
    }
}
