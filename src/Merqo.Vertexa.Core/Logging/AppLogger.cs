namespace Merqo.Vertexa.Core.Logging;

/// <summary>
/// Minimal rolling file logger. No PII is ever written; diagnostic errors are
/// preserved, never silently discarded.
/// </summary>
public sealed class AppLogger
{
    private readonly string _logDir;
    private readonly object _gate = new();

    public AppLogger(string logDir)
    {
        _logDir = logDir;
        Directory.CreateDirectory(logDir);
    }

    public void Info(string message) => Write("INFO", message);
    public void Warn(string message) => Write("WARN", message);
    public void Error(string message, Exception? ex = null)
        => Write("ERROR", ex is null ? message : $"{message} | {ex.GetType().Name}: {ex.Message}");

    private void Write(string level, string message)
    {
        try
        {
            var file = Path.Combine(_logDir, $"vertexa-{DateTime.UtcNow:yyyyMMdd}.log");
            lock (_gate)
                File.AppendAllText(file, $"{DateTime.UtcNow:o} [{level}] {message}{Environment.NewLine}");
        }
        catch
        {
            // Logging must never crash the app.
        }
    }
}
