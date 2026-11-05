namespace Merqo.Vertexa.Core.Models;

/// <summary>Aggregated snapshot of all hardware/system collectors.</summary>
public sealed class SystemSnapshot
{
    public DateTimeOffset CapturedAt { get; init; } = DateTimeOffset.UtcNow;
    public Dictionary<string, object> Sections { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> Errors { get; init; } = new();
}

public sealed class CpuInfo
{
    public DiagnosticResult<string> Manufacturer { get; init; } = DiagnosticResult<string>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<string> Model { get; init; } = DiagnosticResult<string>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<string> Architecture { get; init; } = DiagnosticResult<string>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<int> PhysicalCores { get; init; } = DiagnosticResult<int>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<int> LogicalProcessors { get; init; } = DiagnosticResult<int>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<double> BaseFrequencyMHz { get; init; } = DiagnosticResult<double>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<double> MaxFrequencyMHz { get; init; } = DiagnosticResult<double>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<double> CurrentLoadPercent { get; init; } = DiagnosticResult<double>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<double> TemperatureCelsius { get; init; } = DiagnosticResult<double>.Unavailable("none", "none", "not collected");
}

public sealed class GpuInfo
{
    public string Name { get; init; } = "";
    public DiagnosticResult<string> DriverVersion { get; init; } = DiagnosticResult<string>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<long> DedicatedMemoryBytes { get; init; } = DiagnosticResult<long>.Unavailable("none", "none", "not collected");
}

public sealed record RamInfo
{
    public DiagnosticResult<long> TotalBytes { get; init; } = DiagnosticResult<long>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<long> AvailableBytes { get; init; } = DiagnosticResult<long>.Unavailable("none", "none", "not collected");
    public List<PhysicalMemoryModule> Modules { get; init; } = new();
}

public sealed class PhysicalMemoryModule
{
    public string? Manufacturer { get; init; }
    public string? PartNumber { get; init; }
    public long CapacityBytes { get; init; }
    public int ConfiguredSpeedMHz { get; init; }
    public string? MemoryType { get; init; }
}

public sealed class BatteryInfo
{
    public DiagnosticResult<long> DesignCapacityMWh { get; init; } = DiagnosticResult<long>.Unavailable("none", "none", "no battery or not exposed");
    public DiagnosticResult<long> FullChargeCapacityMWh { get; init; } = DiagnosticResult<long>.Unavailable("none", "none", "no battery or not exposed");
    public DiagnosticResult<int> CycleCount { get; init; } = DiagnosticResult<int>.Unavailable("none", "none", "firmware does not expose cycle count");
    public DiagnosticResult<double> WearPercent { get; init; } = DiagnosticResult<double>.Unavailable("none", "none", "requires valid design and full-charge capacity");
    public DiagnosticResult<double> HealthPercent { get; init; } = DiagnosticResult<double>.Unavailable("none", "none", "requires valid design and full-charge capacity");
    public DiagnosticResult<int> ChargePercent { get; init; } = DiagnosticResult<int>.Unavailable("none", "none", "not exposed");
    public bool IsPresent { get; init; }
}

public sealed class StorageDeviceInfo
{
    public string Model { get; init; } = "";
    public string? Serial { get; init; }
    public long CapacityBytes { get; init; }
    public string MediaType { get; init; } = "Unknown";
    public List<PartitionInfo> Partitions { get; init; } = new();
}

public sealed class PartitionInfo
{
    public string DriveLetter { get; init; } = "";
    public string? FileSystem { get; init; }
    public long TotalBytes { get; init; }
    public long FreeBytes { get; init; }
}

public sealed class WindowsInfo
{
    public DiagnosticResult<string> Edition { get; init; } = DiagnosticResult<string>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<string> Version { get; init; } = DiagnosticResult<string>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<string> Build { get; init; } = DiagnosticResult<string>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<string> Architecture { get; init; } = DiagnosticResult<string>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<DateTimeOffset> UptimeSince { get; init; } = DiagnosticResult<DateTimeOffset>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<string> SecureBoot { get; init; } = DiagnosticResult<string>.Unavailable("none", "none", "not collected");
    public DiagnosticResult<string> Tpm { get; init; } = DiagnosticResult<string>.Unavailable("none", "none", "not collected");
}

public sealed class DriverIssue
{
    public string DeviceName { get; init; } = "";
    public uint ProblemCode { get; init; }
    public string? HardwareId { get; init; }
}
