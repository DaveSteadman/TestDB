using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TestDB.Server.Data;
using TestDB.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add command-line configuration
builder.Configuration.AddCommandLine(args);

// Get configuration values (command-line overrides appsettings)
var jwtSecret = builder.Configuration["jwt-secret"] 
    ?? builder.Configuration["Jwt:Secret"] 
    ?? "your-secret-key-change-in-production";

var corsOrigin = builder.Configuration["cors-origin"] 
    ?? builder.Configuration["Cors:AllowedOrigin"] 
    ?? "http://localhost:3000";

var port = builder.Configuration["port"] 
    ?? builder.Configuration["p"] 
    ?? builder.Configuration["Server:Port"] 
    ?? "3001";

var environment = builder.Configuration["environment"] 
    ?? builder.Configuration["e"] 
    ?? builder.Environment.EnvironmentName;

// Update environment if specified
if (builder.Configuration["environment"] != null || builder.Configuration["e"] != null)
{
    builder.Environment.EnvironmentName = environment;
}

// Warning for production
if (jwtSecret == "your-secret-key-change-in-production" && 
    builder.Environment.IsProduction())
{
    Console.WriteLine("WARNING: Using default JWT secret in production is insecure! Set JWT_SECRET environment variable or use --jwt-secret parameter.");
}

// Configure port
builder.WebHost.UseUrls($"http://localhost:{port}");

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Add DbContext
var connectionString = builder.Configuration["Database:ConnectionString"] ?? "Data Source=testdb.db";
builder.Services.AddDbContext<TestDbContext>(options =>
    options.UseSqlite(connectionString));

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();

// Build app
var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TestDbContext>();
        var dbLogger = services.GetRequiredService<ILogger<Program>>();
        DbInitializer.Initialize(context, dbLogger);
    }
    catch (Exception ex)
    {
        var errorLogger = app.Services.GetRequiredService<ILogger<Program>>();
        errorLogger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure middleware pipeline
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation($"Server running on port {port}");
logger.LogInformation($"API available at http://localhost:{port}/api");
logger.LogInformation($"Environment: {builder.Environment.EnvironmentName}");
logger.LogInformation($"CORS origin: {corsOrigin}");

app.Run();
