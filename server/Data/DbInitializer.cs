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
            var adminPassword = BCrypt.Net.BCrypt.HashPassword("admin");
            var adminUser = context.Users.FirstOrDefault(u => u.Username == "admin");

            if (adminUser == null)
            {
                adminUser = new User
                {
                    Username = "admin",
                    Password = adminPassword,
                    Email = "admin@testdb.com",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
                logger.LogInformation("Admin user created (username: admin, password: admin)");
            }
            else
            {
                adminUser.Password = adminPassword;
                if (string.IsNullOrWhiteSpace(adminUser.Email))
                {
                    adminUser.Email = "admin@testdb.com";
                }
                context.SaveChanges();
                logger.LogInformation("Admin user updated (username: admin, password: admin)");
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
