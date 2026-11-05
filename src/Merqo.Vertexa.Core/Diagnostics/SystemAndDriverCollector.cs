using Merqo.Vertexa.Core.Models;
using Merqo.Vertexa.Core.Providers;

namespace Merqo.Vertexa.Core.Diagnostics;

public static class SystemInfoCollector
{
    public static WindowsInfo Collect()
    {
        try
        {
            var os = WmiProvider.Query("Win32_OperatingSystem", "Caption", "Version", "BuildNumber", "OSArchitecture", "LastBootUpTime").FirstOrDefault()
                     ?? new Dictionary<string, object?>();
            var boot = DateTime.TryParse(WmiProvider.GetString(os, "LastBootUpTime"), out var b) ? b : (DateTime?)null;

            var bb = WmiProvider.Query("Win32_BaseBoard", "Manufacturer", "Product", "Version").FirstOrDefault() ?? new Dictionary<string, object?>();
            var bios = WmiProvider.Query("Win32_BIOS", "SMBIOSBIOSVersion", "ReleaseDate").FirstOrDefault() ?? new Dictionary<string, object?>();

            var secureBoot = "Unknown";
            try
            {
                var (code, stdout, _) = ProcessProvider.Run("powershell.exe",
                    new[] { "-NoProfile", "-Command", "try { Confirm-SecureBootUEFI } catch { 'Unknown' }" });
                secureBoot = code == 0 ? stdout.Trim() : "Unknown";
            }
            catch { secureBoot = "Unknown"; }

            return new WindowsInfo
            {
                Edition = DiagnosticResult<string>.Ok(WmiProvider.GetString(os, "Caption") ?? "Unknown", "WMI", "Win32_OperatingSystem.Caption"),
                Version = DiagnosticResult<string>.Ok(WmiProvider.GetString(os, "Version") ?? "Unknown", "WMI", "Win32_OperatingSystem.Version"),
                Build = DiagnosticResult<string>.Ok(WmiProvider.GetString(os, "BuildNumber") ?? "Unknown", "WMI", "Win32_OperatingSystem.BuildNumber"),
                Architecture = DiagnosticResult<string>.Ok(WmiProvider.GetString(os, "OSArchitecture") ?? "Unknown", "WMI", "Win32_OperatingSystem.OSArchitecture"),
                UptimeSince = boot is not null
                    ? DiagnosticResult<DateTimeOffset>.Ok(new DateTimeOffset(boot.Value, TimeSpan.Zero), "WMI", "Win32_OperatingSystem.LastBootUpTime")
                    : DiagnosticResult<DateTimeOffset>.Unavailable("WMI", "LastBootUpTime", "Could not parse boot time"),
                SecureBoot = DiagnosticResult<string>.Ok(secureBoot, "PowerShell", "Confirm-SecureBootUEFI (read-only)", confidence: 0.9),
                Tpm = DiagnosticResult<string>.Ok(
                    WmiProvider.Query("Win32_Tpm", "IsActivated_InitialValue", "SpecVersion").FirstOrDefault() is { } t
                        ? $"Present (activated: {WmiProvider.GetString(t, "IsActivated_InitialValue")})"
                        : "Not present",
                    "WMI", "Win32_Tpm"),
            };
        }
        catch (Exception ex)
        {
            return new WindowsInfo
            {
                Edition = DiagnosticResult<string>.Failed("WMI", "Win32_OperatingSystem", ex.Message),
            };
        }
    }
}

public static class DriverCollector
{
    private static readonly Dictionary<uint, string> ProblemCodes = new()
    {
        [1] = "Device is not configured correctly",
        [10] = "Device cannot start",
        [22] = "Device is disabled",
        [28] = "Drivers are not installed",
        [31] = "Device is not working properly",
        [32] = "Windows cannot load the drivers",
        [37] = "Windows cannot find the drivers",
        [39] = "Corrupt driver or missing driver file",
        [43] = "Device has been stopped",
    };

    public static List<DriverIssue> CollectIssues()
    {
        var issues = new List<DriverIssue>();
        try
        {
            foreach (var r in WmiProvider.Query("Win32_PnPEntity", "Name", "ConfigManagerErrorCode", "PNPDeviceID"))
            {
                var code = (uint)(WmiProvider.GetInt64(r, "ConfigManagerErrorCode") ?? 0);
                if (code == 0) continue;
                issues.Add(new DriverIssue
                {
                    DeviceName = WmiProvider.GetString(r, "Name") ?? "Unknown device",
                    ProblemCode = code,
                    HardwareId = WmiProvider.GetString(r, "PNPDeviceID"),
                });
            }
        }
        catch (Exception ex)
        {
            issues.Add(new DriverIssue { DeviceName = $"Driver scan failed: {ex.Message}", ProblemCode = 0 });
        }
        return issues;
    }

    public static string DescribeProblem(uint code)
        => ProblemCodes.TryGetValue(code, out var d) ? $"Code {code}: {d}" : $"Code {code}";
}
