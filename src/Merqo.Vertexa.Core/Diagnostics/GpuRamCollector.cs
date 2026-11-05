using Merqo.Vertexa.Core.Models;
using Merqo.Vertexa.Core.Providers;

namespace Merqo.Vertexa.Core.Diagnostics;

public static class GpuCollector
{
    public static List<GpuInfo> Collect()
    {
        var list = new List<GpuInfo>();
        try
        {
            foreach (var r in WmiProvider.Query("Win32_VideoController", "Name", "DriverVersion", "AdapterRAM"))
            {
                var ram = WmiProvider.GetInt64(r, "AdapterRAM");
                list.Add(new GpuInfo
                {
                    Name = WmiProvider.GetString(r, "Name") ?? "Unknown GPU",
                    DriverVersion = DiagnosticResult<string>.Ok(WmiProvider.GetString(r, "DriverVersion") ?? "Unknown", "WMI", "Win32_VideoController.DriverVersion"),
                    // AdapterRAM is 32-bit and wraps above 4GB; report only when plausible.
                    DedicatedMemoryBytes = ram is > 0 and < 4L * 1024 * 1024 * 1024
                        ? DiagnosticResult<long>.PartialOk(ram.Value, "WMI", "Win32_VideoController.AdapterRAM", "32-bit field; values >=4GB wrap")
                        : DiagnosticResult<long>.Unsupported("WMI", "Win32_VideoController.AdapterRAM", "Value implausible (32-bit wrap); not fabricated"),
                });
            }
        }
        catch (Exception ex)
        {
            list.Add(new GpuInfo { Name = "Detection failed", DriverVersion = DiagnosticResult<string>.Failed("WMI", "Win32_VideoController", ex.Message) });
        }
        return list;
    }
}

public static class RamCollector
{
    private static readonly Dictionary<int, string> SmbiosMemoryTypes = new()
    {
        [20] = "DDR", [21] = "DDR2", [24] = "DDR3", [26] = "DDR4", [34] = "DDR5", [35] = "LPDDR5",
    };

    public static RamInfo Collect()
    {
        var info = new RamInfo();
        try
        {
            var mem = new NativeMemory.MemoryStatusEx().Init();
            if (NativeMemory.GlobalMemoryStatusEx(ref mem))
            {
                info = info with
                {
                    TotalBytes = DiagnosticResult<long>.Ok((long)mem.ullTotalPhys, "Native", "GlobalMemoryStatusEx.ullTotalPhys", "bytes"),
                    AvailableBytes = DiagnosticResult<long>.Ok((long)mem.ullAvailPhys, "Native", "GlobalMemoryStatusEx.ullAvailPhys", "bytes"),
                };
            }
            foreach (var r in WmiProvider.Query("Win32_PhysicalMemory", "Manufacturer", "PartNumber", "Capacity", "ConfiguredClockSpeed", "SMBIOSMemoryType"))
            {
                var typeId = (int)(WmiProvider.GetInt64(r, "SMBIOSMemoryType") ?? 0);
                info.Modules.Add(new PhysicalMemoryModule
                {
                    Manufacturer = WmiProvider.GetString(r, "Manufacturer"),
                    PartNumber = WmiProvider.GetString(r, "PartNumber"),
                    CapacityBytes = WmiProvider.GetInt64(r, "Capacity") ?? 0,
                    ConfiguredSpeedMHz = (int)(WmiProvider.GetInt64(r, "ConfiguredClockSpeed") ?? 0),
                    MemoryType = SmbiosMemoryTypes.GetValueOrDefault(typeId, typeId > 0 ? $"Type {typeId}" : null),
                });
            }
        }
        catch (Exception ex)
        {
            info = info with { TotalBytes = DiagnosticResult<long>.Failed("WMI/native", "memory query", ex.Message) };
        }
        return info;
    }
}
