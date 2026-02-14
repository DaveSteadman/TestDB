using TestDB.Server.Models;

namespace TestDB.Server.Services;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Generates a JWT token for the specified user
    /// </summary>
    string GenerateToken(User user);
}
