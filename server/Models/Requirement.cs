namespace TestDB.Server.Models;

/// <summary>
/// Priority levels for requirements
/// </summary>
public enum Priority
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Status values for requirements
/// </summary>
public enum RequirementStatus
{
    Draft,
    Active,
    Completed,
    Deprecated
}

/// <summary>
/// Represents a software requirement
/// </summary>
public class Requirement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public RequirementStatus Status { get; set; } = RequirementStatus.Draft;
    public int CreatedBy { get; set; }
    public User? Creator { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
