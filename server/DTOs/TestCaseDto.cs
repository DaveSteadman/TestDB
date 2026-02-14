using TestDB.Server.Models;

namespace TestDB.Server.DTOs;

/// <summary>
/// Data transfer object for test case requests
/// </summary>
public class TestCaseDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Steps { get; set; }
    public string? ExpectedResult { get; set; }
    public string? Status { get; set; }
}
