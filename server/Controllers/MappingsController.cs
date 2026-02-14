using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestDB.Server.Data;
using TestDB.Server.Models;

namespace TestDB.Server.Controllers;

/// <summary>
/// Mappings controller for requirement-test case relationships
/// </summary>
[ApiController]
[Route("api/mappings")]
[Authorize]
public class MappingsController : ControllerBase
{
    private readonly TestDbContext _context;
    private readonly ILogger<MappingsController> _logger;

    public MappingsController(TestDbContext context, ILogger<MappingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all mappings with requirement and test case titles
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var mappings = await _context.RequirementTestMappings
                .Include(m => m.Requirement)
                .Include(m => m.TestCase)
                .Select(m => new
                {
                    id = m.Id,
                    requirement_id = m.RequirementId,
                    test_case_id = m.TestCaseId,
                    requirement_title = m.Requirement != null ? m.Requirement.Title : null,
                    test_case_title = m.TestCase != null ? m.TestCase.Title : null,
                    created_at = m.CreatedAt
                })
                .ToListAsync();

            return Ok(mappings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching mappings");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Create a new mapping
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMappingDto dto)
    {
        try
        {
            var mapping = new RequirementTestMapping
            {
                RequirementId = dto.RequirementId,
                TestCaseId = dto.TestCaseId,
                CreatedAt = DateTime.UtcNow
            };

            _context.RequirementTestMappings.Add(mapping);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { id = mapping.Id, message = "Mapping created successfully" });
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                return BadRequest(new { error = "Mapping already exists" });
            }
            _logger.LogError(ex, "Error creating mapping");
            return StatusCode(500, new { error = "Database error" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating mapping");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Delete a mapping
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var mapping = await _context.RequirementTestMappings.FindAsync(id);

            if (mapping == null)
            {
                return NotFound(new { error = "Mapping not found" });
            }

            _context.RequirementTestMappings.Remove(mapping);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Mapping deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting mapping");
            return StatusCode(500, new { error = "Database error" });
        }
    }
}

/// <summary>
/// DTO for creating a mapping
/// </summary>
public class CreateMappingDto
{
    public int RequirementId { get; set; }
    public int TestCaseId { get; set; }
}
