using System.Text;

namespace TestDB.Server.Services;

/// <summary>
/// Simple file-based event logger for audit-style events.
/// </summary>
public class EventsLogger
{
    private readonly string _logPath;
    private readonly object _lock = new();

    public EventsLogger(IHostEnvironment environment)
    {
        _logPath = Path.Combine(environment.ContentRootPath, "events.log");
    }

    public void Log(string eventType, string message)
    {
        var timestamp = DateTime.UtcNow.ToString("o");
        var line = new StringBuilder()
            .Append(timestamp)
            .Append(' ')
            .Append(eventType)
            .Append(' ')
            .Append(message)
            .AppendLine()
            .ToString();

        lock (_lock)
        {
            File.AppendAllText(_logPath, line);
        }
    }
}
