using Microsoft.AspNetCore.Mvc;

namespace TestDB.Server.Controllers;

/// <summary>
/// Health check controller
/// </summary>
[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Check server health status
    /// </summary>
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new 
        { 
            status = "ok", 
            timestamp = DateTime.UtcNow.ToString("o")
        });
    }
}
