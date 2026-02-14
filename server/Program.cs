using System.Text;
using System.Diagnostics;
using System.Security.Claims;
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
    ?? "http://localhost:3000,http://localhost:3003";

var corsOrigins = corsOrigin
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(origin => origin.Trim())
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .ToArray();

var port = builder.Configuration["port"]
    ?? builder.Configuration["p"]
    ?? builder.Configuration["Server:Port"]
    ?? builder.Configuration["PORT"]
    ?? "3001";

var environment = builder.Configuration["environment"]
    ?? builder.Configuration["e"]
    ?? builder.Environment.EnvironmentName;

var devBypassAuth = builder.Configuration.GetValue<bool?>("dev-bypass-auth")
    ?? builder.Configuration.GetValue<bool?>("DEV_BYPASS_AUTH")
    ?? false;

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

// Configure URLs (do not override if URLs are explicitly configured)
var urls = builder.Configuration["urls"]
    ?? builder.Configuration["ASPNETCORE_URLS"];

var listenUrls = string.IsNullOrWhiteSpace(urls)
    ? $"http://localhost:{port}"
    : urls;

builder.WebHost.UseUrls(listenUrls);

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
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<EventsLogger>();

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
if (devBypassAuth)
{
    app.Use(async (context, next) =>
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            var claims = new[]
            {
                new Claim("id", "1"),
                new Claim("username", "admin"),
                new Claim("email", "admin@testdb.com")
            };
            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "DevBypass"));

            var eventsLogger = context.RequestServices.GetRequiredService<EventsLogger>();
            eventsLogger.Log("AUTH_BYPASS", $"path={context.Request.Path}");
        }

        await next();
    });
}
app.UseAuthorization();
app.Use(async (context, next) =>
{
    var eventsLogger = context.RequestServices.GetRequiredService<EventsLogger>();
    var stopwatch = Stopwatch.StartNew();
    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var username = context.User?.FindFirst("username")?.Value ?? "anonymous";

    eventsLogger.Log("REQUEST_START", $"{context.Request.Method} {context.Request.Path} from {ip} user={username}");
    await next();
    stopwatch.Stop();
    eventsLogger.Log("REQUEST_END", $"{context.Request.Method} {context.Request.Path} {context.Response.StatusCode} {stopwatch.ElapsedMilliseconds}ms from {ip} user={username}");
});
app.MapControllers();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var primaryUrl = listenUrls.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim()
    ?? $"http://localhost:{port}";
logger.LogInformation($"Server running on {listenUrls}");
logger.LogInformation($"API available at {primaryUrl}/api");
logger.LogInformation($"Environment: {builder.Environment.EnvironmentName}");
logger.LogInformation($"CORS origin(s): {string.Join(", ", corsOrigins)}");
logger.LogInformation($"Dev auth bypass: {devBypassAuth}");

var eventsLogger = app.Services.GetRequiredService<EventsLogger>();
eventsLogger.Log("SERVER_START", $"Server running on {listenUrls}; env={builder.Environment.EnvironmentName}; devBypassAuth={devBypassAuth}");

app.Run();
