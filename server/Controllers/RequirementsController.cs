using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestDB.Server.Data;
using TestDB.Server.DTOs;
using TestDB.Server.Models;

namespace TestDB.Server.Controllers;

/// <summary>
/// Requirements controller for CRUD operations
/// </summary>
[ApiController]
[Route("api/requirements")]
[Authorize]
public class RequirementsController : ControllerBase
{
    private readonly TestDbContext _context;
    private readonly ILogger<RequirementsController> _logger;

    public RequirementsController(TestDbContext context, ILogger<RequirementsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    /// <summary>
    /// Get all requirements with creator username
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var requirements = await _context.Requirements
                .Include(r => r.Creator)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    id = r.Id,
                    title = r.Title,
                    description = r.Description,
                    priority = r.Priority.ToString(),
                    status = r.Status.ToString(),
                    created_by = r.CreatedBy,
                    creator = r.Creator != null ? r.Creator.Username : null,
                    created_at = r.CreatedAt,
                    updated_at = r.UpdatedAt
                })
                .ToListAsync();

            return Ok(requirements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching requirements");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Get a specific requirement by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var requirement = await _context.Requirements.FindAsync(id);

            if (requirement == null)
            {
                return NotFound(new { error = "Requirement not found" });
            }

            return Ok(new
            {
                id = requirement.Id,
                title = requirement.Title,
                description = requirement.Description,
                priority = requirement.Priority.ToString(),
                status = requirement.Status.ToString(),
                created_by = requirement.CreatedBy,
                created_at = requirement.CreatedAt,
                updated_at = requirement.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching requirement");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Create a new requirement
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RequirementDto dto)
    {
        try
        {
            var priority = string.IsNullOrWhiteSpace(dto.Priority) 
                ? Priority.Medium 
                : Enum.Parse<Priority>(dto.Priority, true);
            
            var status = string.IsNullOrWhiteSpace(dto.Status) 
                ? RequirementStatus.Draft 
                : Enum.Parse<RequirementStatus>(dto.Status, true);

            var requirement = new Requirement
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = priority,
                Status = status,
                CreatedBy = GetUserId(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Requirements.Add(requirement);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { id = requirement.Id, message = "Requirement created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating requirement");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Update a requirement
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] RequirementDto dto)
    {
        try
        {
            var requirement = await _context.Requirements.FindAsync(id);

            if (requirement == null)
            {
                return NotFound(new { error = "Requirement not found" });
            }

            requirement.Title = dto.Title;
            requirement.Description = dto.Description;
            requirement.Priority = string.IsNullOrWhiteSpace(dto.Priority) 
                ? requirement.Priority 
                : Enum.Parse<Priority>(dto.Priority, true);
            requirement.Status = string.IsNullOrWhiteSpace(dto.Status) 
                ? requirement.Status 
                : Enum.Parse<RequirementStatus>(dto.Status, true);
            requirement.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Requirement updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating requirement");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Delete a requirement
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var requirement = await _context.Requirements.FindAsync(id);

            if (requirement == null)
            {
                return NotFound(new { error = "Requirement not found" });
            }

            _context.Requirements.Remove(requirement);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Requirement deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting requirement");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Get all test cases linked to a requirement
    /// </summary>
    [HttpGet("{id}/test-cases")]
    public async Task<IActionResult> GetTestCases(int id)
    {
        try
        {
            var testCases = await _context.RequirementTestMappings
                .Where(m => m.RequirementId == id)
                .Include(m => m.TestCase)
                .Select(m => new
                {
                    id = m.TestCase!.Id,
                    title = m.TestCase.Title,
                    description = m.TestCase.Description,
                    steps = m.TestCase.Steps,
                    expected_result = m.TestCase.ExpectedResult,
                    status = m.TestCase.Status.ToString(),
                    created_by = m.TestCase.CreatedBy,
                    created_at = m.TestCase.CreatedAt,
                    updated_at = m.TestCase.UpdatedAt
                })
                .ToListAsync();

            return Ok(testCases);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching test cases for requirement");
            return StatusCode(500, new { error = "Database error" });
        }
    }
}
