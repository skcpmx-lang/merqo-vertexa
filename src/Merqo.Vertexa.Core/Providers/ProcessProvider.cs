using System.Diagnostics;

namespace Merqo.Vertexa.Core.Providers;

/// <summary>
/// Runs fixed, allow-listed read-only CLI tools (e.g. powercfg) with argument
/// arrays — no shell, no string interpolation of untrusted input.
/// </summary>
public static class ProcessProvider
{
    public static (int ExitCode, string StdOut, string StdErr) Run(string fileName, IReadOnlyList<string> args, int timeoutMs = 30_000)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            foreach (var a in args) psi.ArgumentList.Add(a);

            using var proc = Process.Start(psi) ?? throw new ProviderException($"Failed to start {fileName}");
            var stdout = proc.StandardOutput.ReadToEndAsync();
            var stderr = proc.StandardError.ReadToEndAsync();
            if (!proc.WaitForExit(timeoutMs))
            {
                try { proc.Kill(entireProcessTree: true); } catch { /* best effort */ }
                throw new ProviderException($"{fileName} timed out after {timeoutMs} ms");
            }
            return (proc.ExitCode, stdout.GetAwaiter().GetResult(), stderr.GetAwaiter().GetResult());
        }
        catch (ProviderException) { throw; }
        catch (Exception ex)
        {
            throw new ProviderException($"Failed to run {fileName}: {ex.Message}", ex);
        }
    }
}
