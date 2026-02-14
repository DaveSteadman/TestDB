using Microsoft.AspNetCore.Mvc;
using TestServer.Models;
using TestServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Register DatabaseService as a singleton
var connectionString = "Data Source=testdb.sqlite";
builder.Services.AddSingleton(new DatabaseService(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// User endpoints
app.MapGet("/api/users", async (DatabaseService db) => 
{
    var users = await db.GetUsersAsync();
    return Results.Ok(users);
});

app.MapGet("/api/users/{id}", async (int id, DatabaseService db) => 
{
    var user = await db.GetUserByIdAsync(id);
    return user != null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/api/users", async ([FromBody] UserCreateRequest request, DatabaseService db) => 
{
    var user = await db.CreateUserAsync(request.Name);
    return Results.Created($"/api/users/{user.Id}", user);
});

app.MapPut("/api/users/{id}", async (int id, [FromBody] UserCreateRequest request, DatabaseService db) => 
{
    var success = await db.UpdateUserAsync(id, request.Name);
    return success ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/api/users/{id}", async (int id, DatabaseService db) => 
{
    var success = await db.DeleteUserAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
});

// Requirement endpoints
app.MapGet("/api/requirements", async (DatabaseService db) => 
{
    var requirements = await db.GetRequirementsAsync();
    return Results.Ok(requirements);
});

app.MapGet("/api/requirements/{id}", async (int id, DatabaseService db) => 
{
    var requirement = await db.GetRequirementByIdAsync(id);
    return requirement != null ? Results.Ok(requirement) : Results.NotFound();
});

app.MapPost("/api/requirements", async ([FromBody] RequirementCreateRequest request, DatabaseService db) => 
{
    var requirement = await db.CreateRequirementAsync(request.Text);
    return Results.Created($"/api/requirements/{requirement.Id}", requirement);
});

app.MapPut("/api/requirements/{id}", async (int id, [FromBody] RequirementCreateRequest request, DatabaseService db) => 
{
    var success = await db.UpdateRequirementAsync(id, request.Text);
    return success ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/api/requirements/{id}", async (int id, DatabaseService db) => 
{
    var success = await db.DeleteRequirementAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
});

// TestCase endpoints
app.MapGet("/api/testcases", async (DatabaseService db) => 
{
    var testCases = await db.GetTestCasesAsync();
    return Results.Ok(testCases);
});

app.MapGet("/api/testcases/{id}", async (int id, DatabaseService db) => 
{
    var testCase = await db.GetTestCaseByIdAsync(id);
    return testCase != null ? Results.Ok(testCase) : Results.NotFound();
});

app.MapPost("/api/testcases", async ([FromBody] TestCaseCreateRequest request, DatabaseService db) => 
{
    var testCase = await db.CreateTestCaseAsync(request.Text);
    return Results.Created($"/api/testcases/{testCase.Id}", testCase);
});

app.MapPut("/api/testcases/{id}", async (int id, [FromBody] TestCaseCreateRequest request, DatabaseService db) => 
{
    var success = await db.UpdateTestCaseAsync(id, request.Text);
    return success ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/api/testcases/{id}", async (int id, DatabaseService db) => 
{
    var success = await db.DeleteTestCaseAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
});

// RequirementTestCase mapping endpoints
app.MapGet("/api/mappings", async (DatabaseService db) => 
{
    var mappings = await db.GetRequirementTestCasesAsync();
    return Results.Ok(mappings);
});

app.MapGet("/api/requirements/{id}/testcases", async (int id, DatabaseService db) => 
{
    var testCases = await db.GetTestCasesForRequirementAsync(id);
    return Results.Ok(testCases);
});

app.MapGet("/api/testcases/{id}/requirements", async (int id, DatabaseService db) => 
{
    var requirements = await db.GetRequirementsForTestCaseAsync(id);
    return Results.Ok(requirements);
});

app.MapPost("/api/mappings", async ([FromBody] MappingRequest request, DatabaseService db) => 
{
    var success = await db.AddRequirementTestCaseMappingAsync(request.RequirementId, request.TestCaseId);
    return success ? Results.Created("/api/mappings", request) : Results.Conflict();
});

app.MapDelete("/api/mappings", async ([FromBody] MappingRequest request, DatabaseService db) => 
{
    var success = await db.RemoveRequirementTestCaseMappingAsync(request.RequirementId, request.TestCaseId);
    return success ? Results.NoContent() : Results.NotFound();
});

Console.WriteLine("Test Server is starting...");
Console.WriteLine("SQLite database with semaphore-protected access");
Console.WriteLine("API endpoints available at:");
Console.WriteLine("  - GET/POST    /api/users");
Console.WriteLine("  - GET/PUT/DEL /api/users/{id}");
Console.WriteLine("  - GET/POST    /api/requirements");
Console.WriteLine("  - GET/PUT/DEL /api/requirements/{id}");
Console.WriteLine("  - GET/POST    /api/testcases");
Console.WriteLine("  - GET/PUT/DEL /api/testcases/{id}");
Console.WriteLine("  - GET/POST/DEL /api/mappings");
Console.WriteLine("  - GET /api/requirements/{id}/testcases");
Console.WriteLine("  - GET /api/testcases/{id}/requirements");

app.Run();

// Request DTOs
record UserCreateRequest(string Name);
record RequirementCreateRequest(string Text);
record TestCaseCreateRequest(string Text);
record MappingRequest(int RequirementId, int TestCaseId);
