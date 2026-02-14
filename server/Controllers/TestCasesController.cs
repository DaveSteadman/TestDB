using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestDB.Server.Data;
using TestDB.Server.DTOs;
using TestDB.Server.Models;

namespace TestDB.Server.Controllers;

/// <summary>
/// Test cases controller for CRUD operations
/// </summary>
[ApiController]
[Route("api/test-cases")]
[Authorize]
public class TestCasesController : ControllerBase
{
    private readonly TestDbContext _context;
    private readonly ILogger<TestCasesController> _logger;

    public TestCasesController(TestDbContext context, ILogger<TestCasesController> logger)
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
    /// Get all test cases with creator username
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var testCases = await _context.TestCases
                .Include(t => t.Creator)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new
                {
                    id = t.Id,
                    title = t.Title,
                    description = t.Description,
                    steps = t.Steps,
                    expected_result = t.ExpectedResult,
                    status = t.Status.ToString(),
                    created_by = t.CreatedBy,
                    creator = t.Creator != null ? t.Creator.Username : null,
                    created_at = t.CreatedAt,
                    updated_at = t.UpdatedAt
                })
                .ToListAsync();

            return Ok(testCases);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching test cases");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Get a specific test case by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var testCase = await _context.TestCases.FindAsync(id);

            if (testCase == null)
            {
                return NotFound(new { error = "Test case not found" });
            }

            return Ok(new
            {
                id = testCase.Id,
                title = testCase.Title,
                description = testCase.Description,
                steps = testCase.Steps,
                expected_result = testCase.ExpectedResult,
                status = testCase.Status.ToString(),
                created_by = testCase.CreatedBy,
                created_at = testCase.CreatedAt,
                updated_at = testCase.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching test case");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Create a new test case
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TestCaseDto dto)
    {
        try
        {
            var status = string.IsNullOrWhiteSpace(dto.Status) 
                ? TestCaseStatus.Draft 
                : Enum.Parse<TestCaseStatus>(dto.Status, true);

            var testCase = new TestCase
            {
                Title = dto.Title,
                Description = dto.Description,
                Steps = dto.Steps,
                ExpectedResult = dto.ExpectedResult,
                Status = status,
                CreatedBy = GetUserId(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TestCases.Add(testCase);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { id = testCase.Id, message = "Test case created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating test case");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Update a test case
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TestCaseDto dto)
    {
        try
        {
            var testCase = await _context.TestCases.FindAsync(id);

            if (testCase == null)
            {
                return NotFound(new { error = "Test case not found" });
            }

            testCase.Title = dto.Title;
            testCase.Description = dto.Description;
            testCase.Steps = dto.Steps;
            testCase.ExpectedResult = dto.ExpectedResult;
            testCase.Status = string.IsNullOrWhiteSpace(dto.Status) 
                ? testCase.Status 
                : Enum.Parse<TestCaseStatus>(dto.Status, true);
            testCase.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Test case updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating test case");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Delete a test case
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var testCase = await _context.TestCases.FindAsync(id);

            if (testCase == null)
            {
                return NotFound(new { error = "Test case not found" });
            }

            _context.TestCases.Remove(testCase);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Test case deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting test case");
            return StatusCode(500, new { error = "Database error" });
        }
    }

    /// <summary>
    /// Get all requirements linked to a test case
    /// </summary>
    [HttpGet("{id}/requirements")]
    public async Task<IActionResult> GetRequirements(int id)
    {
        try
        {
            var requirements = await _context.RequirementTestMappings
                .Where(m => m.TestCaseId == id)
                .Include(m => m.Requirement)
                .Select(m => new
                {
                    id = m.Requirement!.Id,
                    title = m.Requirement.Title,
                    description = m.Requirement.Description,
                    priority = m.Requirement.Priority.ToString(),
                    status = m.Requirement.Status.ToString(),
                    created_by = m.Requirement.CreatedBy,
                    created_at = m.Requirement.CreatedAt,
                    updated_at = m.Requirement.UpdatedAt
                })
                .ToListAsync();

            return Ok(requirements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching requirements for test case");
            return StatusCode(500, new { error = "Database error" });
        }
    }
}
