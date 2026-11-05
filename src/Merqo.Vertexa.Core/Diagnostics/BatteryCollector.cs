using System.Xml.Linq;
using Merqo.Vertexa.Core.Models;
using Merqo.Vertexa.Core.Providers;

namespace Merqo.Vertexa.Core.Diagnostics;

/// <summary>
/// Battery Intelligence Engine. Wear/health are calculated ONLY when both
/// design and full-charge capacities are valid and positive:
///   Wear%   = (Design - FullCharge) / Design * 100
///   Health% = FullCharge / Design * 100
/// Cycle count is never fabricated when firmware hides it.
/// </summary>
public static class BatteryCollector
{
    public static BatteryInfo Collect()
    {
        var design = WmiLong("BatteryStaticData", "DesignedCapacity");
        var full = WmiLong("BatteryFullChargedCapacity", "FullChargedCapacity");
        if (design is null || full is null)
        {
            // Try powercfg /batteryreport as a secondary source.
            TryPowerCfgBatteryReport(out var pcDesign, out var pcFull, out var pcCycles);
            design ??= pcDesign; full ??= pcFull;
            var cycles = WmiInt("BatteryCycleCount", "CycleCount") ?? pcCycles;
            return Build(design, full, cycles, chargePercent: CurrentChargePercent());
        }
        var cyclesOnly = WmiInt("BatteryCycleCount", "CycleCount");
        return Build(design, full, cyclesOnly, chargePercent: CurrentChargePercent());
    }

    private static BatteryInfo Build(long? design, long? full, int? cycles, int? chargePercent)
    {
        var info = new BatteryInfo { IsPresent = design is not null || full is not null || chargePercent is not null };

        DiagnosticResult<long> Design(long? v) => v is > 0
            ? DiagnosticResult<long>.Ok(v.Value, "WMI/powercfg", "BatteryStaticData.DesignedCapacity", "mWh")
            : DiagnosticResult<long>.Unsupported("WMI/powercfg", "DesignedCapacity", "Not exposed by firmware");
        DiagnosticResult<long> Full(long? v) => v is > 0
            ? DiagnosticResult<long>.Ok(v.Value, "WMI/powercfg", "BatteryFullChargedCapacity.FullChargedCapacity", "mWh")
            : DiagnosticResult<long>.Unsupported("WMI/powercfg", "FullChargedCapacity", "Not exposed by firmware");

        var d = Design(design);
        var f = Full(full);

        DiagnosticResult<double> wear, health;
        if (design is > 0 && full is > 0 && full <= design)
        {
            wear = DiagnosticResult<double>.EstimatedOk((double)(design.Value - full.Value) / design.Value * 100.0,
                "calculation", "(Design - FullCharge)/Design * 100", "Wear = (Design - FullCharge)/Design * 100");
            health = DiagnosticResult<double>.EstimatedOk((double)full.Value / design.Value * 100.0,
                "calculation", "FullCharge/Design * 100", "Health = FullCharge/Design * 100");
        }
        else
        {
            wear = DiagnosticResult<double>.Unavailable("calculation", "wear formula", "Requires valid positive design and full-charge capacity");
            health = DiagnosticResult<double>.Unavailable("calculation", "health formula", "Requires valid positive design and full-charge capacity");
        }

        var cyc = cycles is not null
            ? DiagnosticResult<int>.Ok(cycles.Value, "WMI/powercfg", "BatteryCycleCount.CycleCount")
            : DiagnosticResult<int>.Unsupported("WMI/powercfg", "CycleCount", "Firmware does not expose cycle count; value not fabricated");

        var chg = chargePercent is not null
            ? DiagnosticResult<int>.Ok(chargePercent.Value, "WMI", "Win32_Battery.EstimatedChargeRemaining", "%")
            : DiagnosticResult<int>.Unsupported("WMI", "Win32_Battery", "No battery present or charge not exposed");

        return new BatteryInfo
        {
            IsPresent = info.IsPresent,
            DesignCapacityMWh = d,
            FullChargeCapacityMWh = f,
            CycleCount = cyc,
            WearPercent = wear,
            HealthPercent = health,
            ChargePercent = chg,
        };
    }

    private static long? WmiLong(string cls, string prop)
    {
        try { return WmiProvider.Query(cls, prop).Select(r => WmiProvider.GetInt64(r, prop)).FirstOrDefault(v => v is not null); }
        catch { return null; }
    }

    private static int? WmiInt(string cls, string prop)
    {
        try { return WmiProvider.Query(cls, prop).Select(r => WmiProvider.GetInt64(r, prop)).FirstOrDefault(v => v is not null) is long l ? (int)l : null; }
        catch { return null; }
    }

    private static int? CurrentChargePercent()
    {
        try { return WmiProvider.Query("Win32_Battery", "EstimatedChargeRemaining").Select(r => WmiProvider.GetInt64(r, "EstimatedChargeRemaining")).FirstOrDefault(v => v is >= 0 and <= 100) is long l ? (int)l : null; }
        catch { return null; }
    }

    private static void TryPowerCfgBatteryReport(out long? design, out long? full, out int? cycles)
    {
        design = full = null; cycles = null;
        try
        {
            var path = Path.Combine(Path.GetTempPath(), "merqo-battery-report.xml");
            var (code, _, _) = ProcessProvider.Run("powercfg.exe", new[] { "/batteryreport", "/output", path, "/xml" });
            if (code != 0 || !File.Exists(path)) return;
            var doc = XDocument.Load(path);
            design = ParseLong(doc, "DesignCapacity");
            full = ParseLong(doc, "FullChargeCapacity");
            cycles = ParseInt(doc, "CycleCount");
            try { File.Delete(path); } catch { /* best effort cleanup */ }
        }
        catch { /* secondary source unavailable — primary WMI results still stand */ }
    }

    private static long? ParseLong(XDocument doc, string localName)
        => doc.Descendants().FirstOrDefault(e => e.Name.LocalName == localName)?.Value is string s && long.TryParse(s, out var l) ? l : null;

    private static int? ParseInt(XDocument doc, string localName)
        => doc.Descendants().FirstOrDefault(e => e.Name.LocalName == localName)?.Value is string s && int.TryParse(s, out var i) ? i : null;
}
