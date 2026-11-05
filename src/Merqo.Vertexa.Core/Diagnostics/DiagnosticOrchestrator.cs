using Merqo.Vertexa.Core.Models;

namespace Merqo.Vertexa.Core.Diagnostics;

/// <summary>
/// Runs all module collectors asynchronously with cancellation, timeout and
/// per-module error isolation. A failing module never blocks the others.
/// </summary>
public sealed class DiagnosticOrchestrator
{
    private readonly TimeSpan _moduleTimeout;

    public DiagnosticOrchestrator(TimeSpan? moduleTimeout = null)
        => _moduleTimeout = moduleTimeout ?? TimeSpan.FromSeconds(30);

    public async Task<SystemSnapshot> RunFullScanAsync(IProgress<DiagnosticProgress>? progress = null, CancellationToken ct = default)
    {
        var snapshot = new SystemSnapshot();
        var modules = new (string Name, Func<object> Run)[]
        {
            ("CPU", () => CpuCollector.Collect()),
            ("GPU", () => GpuCollector.Collect()),
            ("RAM", () => RamCollector.Collect()),
            ("Battery", () => BatteryCollector.Collect()),
            ("Storage", () => StorageCollector.Collect()),
            ("Windows", () => SystemInfoCollector.Collect()),
            ("Drivers", () => DriverCollector.CollectIssues()),
        };

        var done = 0;
        foreach (var (name, run) in modules)
        {
            ct.ThrowIfCancellationRequested();
            progress?.Report(new DiagnosticProgress(name, done, modules.Length));
            try
            {
                var result = await Task.Run(run, ct).WaitAsync(_moduleTimeout, ct);
                snapshot.Sections[name] = result;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                // Error isolation: record and continue.
                snapshot.Errors.Add($"{name}: {ex.Message}");
                snapshot.Sections[name] = $"FAILED: {ex.Message}";
            }
            done++;
            progress?.Report(new DiagnosticProgress(name, done, modules.Length));
        }
        return snapshot;
    }
}

public sealed record DiagnosticProgress(string Module, int Completed, int Total)
{
    public double Fraction => Total == 0 ? 0 : (double)Completed / Total;
}
