namespace TestDB.Server.DTOs;

/// <summary>
/// Data transfer object for registration requests
/// </summary>
public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
