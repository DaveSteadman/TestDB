using TestDB.Server.Models;

namespace TestDB.Server.DTOs;

/// <summary>
/// Data transfer object for requirement requests
/// </summary>
public class RequirementDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
}
