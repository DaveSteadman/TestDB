namespace TestDB.Server.Models;

/// <summary>
/// Status values for test cases
/// </summary>
public enum TestCaseStatus
{
    Draft,
    Active,
    Passed,
    Failed,
    Blocked
}

/// <summary>
/// Represents a test case
/// </summary>
public class TestCase
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Steps { get; set; }
    public string? ExpectedResult { get; set; }
    public TestCaseStatus Status { get; set; } = TestCaseStatus.Draft;
    public int CreatedBy { get; set; }
    public User? Creator { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
