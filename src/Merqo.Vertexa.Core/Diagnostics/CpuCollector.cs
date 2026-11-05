using System.Runtime.InteropServices;
using Merqo.Vertexa.Core.Models;
using Merqo.Vertexa.Core.Providers;

namespace Merqo.Vertexa.Core.Diagnostics;

/// <summary>Collects CPU identity and load. Temperature is only reported when
/// the platform genuinely exposes it — otherwise Unsupported, never invented.</summary>
public static class CpuCollector
{
    public static CpuInfo Collect()
    {
        try
        {
            var rows = WmiProvider.Query("Win32_Processor",
                "Manufacturer", "Name", "NumberOfCores", "NumberOfLogicalProcessors",
                "MaxClockSpeed", "CurrentClockSpeed", "LoadPercentage", "AddressWidth");
            var r = rows.FirstOrDefault() ?? new Dictionary<string, object?>();

            var arch = WmiProvider.GetInt64(r, "AddressWidth") switch
            {
                64 => "x64",
                32 => "x86",
                64_9 => "ARM64",
                _ => "Unknown"
            };

            return new CpuInfo
            {
                Manufacturer = DiagnosticResult<string>.Ok(WmiProvider.GetString(r, "Manufacturer") ?? "Unknown", "WMI", "Win32_Processor.Manufacturer"),
                Model = DiagnosticResult<string>.Ok((WmiProvider.GetString(r, "Name") ?? "Unknown").Trim(), "WMI", "Win32_Processor.Name"),
                Architecture = DiagnosticResult<string>.Ok(arch, "WMI", "Win32_Processor.AddressWidth"),
                PhysicalCores = DiagnosticResult<int>.Ok((int)(WmiProvider.GetInt64(r, "NumberOfCores") ?? 0), "WMI", "Win32_Processor.NumberOfCores"),
                LogicalProcessors = DiagnosticResult<int>.Ok((int)(WmiProvider.GetInt64(r, "NumberOfLogicalProcessors") ?? 0), "WMI", "Win32_Processor.NumberOfLogicalProcessors"),
                // MaxClockSpeed is the advertised/base max — labeled accordingly, not a measured boost.
                MaxFrequencyMHz = DiagnosticResult<double>.Ok(WmiProvider.GetDouble(r, "MaxClockSpeed") ?? 0, "WMI", "Win32_Processor.MaxClockSpeed (advertised)", "MHz", 0.8),
                CurrentLoadPercent = DiagnosticResult<double>.Ok(WmiProvider.GetDouble(r, "LoadPercentage") ?? 0, "WMI", "Win32_Processor.LoadPercentage", "%"),
                TemperatureCelsius = DiagnosticResult<double>.Unsupported("WMI MSAcpi_ThermalZoneTemperature", "ACPI thermal zone",
                    "CPU temperature telemetry is not exposed by this hardware/OS; value not fabricated"),
            };
        }
        catch (Exception ex)
        {
            return new CpuInfo
            {
                Model = DiagnosticResult<string>.Failed("WMI", "Win32_Processor", ex.Message),
            };
        }
    }
}

internal static class NativeMemory
{
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx lpBuffer);

    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryStatusEx
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
        public MemoryStatusEx Init() { dwLength = (uint)Marshal.SizeOf(typeof(MemoryStatusEx)); return this; }
    }
}
