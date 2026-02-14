namespace TestDB.Server.Models;

/// <summary>
/// Represents a mapping between a requirement and a test case
/// </summary>
public class RequirementTestMapping
{
    public int Id { get; set; }
    public int RequirementId { get; set; }
    public Requirement? Requirement { get; set; }
    public int TestCaseId { get; set; }
    public TestCase? TestCase { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
