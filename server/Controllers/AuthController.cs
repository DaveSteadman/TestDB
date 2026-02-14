using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestDB.Server.Data;
using TestDB.Server.DTOs;
using TestDB.Server.Models;
using TestDB.Server.Services;

namespace TestDB.Server.Controllers;

/// <summary>
/// Authentication controller for login and registration
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly TestDbContext _context;
    private readonly IAuthService _authService;
    private readonly EventsLogger _eventsLogger;
    private readonly ILogger<AuthController> _logger;

    public AuthController(TestDbContext context, IAuthService authService, EventsLogger eventsLogger, ILogger<AuthController> logger)
    {
        _context = context;
        _authService = authService;
        _eventsLogger = eventsLogger;
        _logger = logger;
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                _eventsLogger.Log("LOGIN_FAILED", $"username={loginDto.Username}");
                return Unauthorized(new { error = "Invalid credentials" });
            }

            var passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);
            if (!passwordValid)
            {
                _eventsLogger.Log("LOGIN_FAILED", $"username={loginDto.Username}");
                return Unauthorized(new { error = "Invalid credentials" });
            }

            var token = _authService.GenerateToken(user);
            _eventsLogger.Log("LOGIN_SUCCESS", $"userId={user.Id} username={user.Username}");

            return Ok(new
            {
                token,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(registerDto.Username) ||
                string.IsNullOrWhiteSpace(registerDto.Password) ||
                string.IsNullOrWhiteSpace(registerDto.Email))
            {
                return BadRequest(new { error = "All fields are required" });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                Username = registerDto.Username,
                Password = hashedPassword,
                Email = registerDto.Email,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _eventsLogger.Log("REGISTER", $"userId={user.Id} username={user.Username}");

            return StatusCode(201, new { message = "User registered successfully", userId = user.Id });
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                return BadRequest(new { error = "Username or email already exists" });
            }
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { error = "Database error" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { error = "Database error" });
        }
    }
}
