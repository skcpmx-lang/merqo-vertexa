using System.Management;

namespace Merqo.Vertexa.Core.Providers;

/// <summary>
/// Thin, safe wrapper over WMI (CIM). Read-only queries only. All calls are
/// wrapped so provider failures surface as diagnostic errors, never crashes.
/// </summary>
public static class WmiProvider
{
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> Query(string wmiClass, params string[] properties)
    {
        var results = new List<IReadOnlyDictionary<string, object?>>();
        try
        {
            using var searcher = new ManagementObjectSearcher($"SELECT {string.Join(",", properties)} FROM {wmiClass}");
            foreach (ManagementObject mo in searcher.Get())
            {
                var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (var p in properties)
                {
                    try { dict[p] = mo[p]; } catch { dict[p] = null; }
                }
                results.Add(dict);
            }
        }
        catch (Exception ex)
        {
            throw new ProviderException($"WMI query failed for {wmiClass}: {ex.Message}", ex);
        }
        return results;
    }

    public static string? GetString(IReadOnlyDictionary<string, object?> row, string key)
        => row.TryGetValue(key, out var v) && v is not null ? v.ToString() : null;

    public static long? GetInt64(IReadOnlyDictionary<string, object?> row, string key)
        => row.TryGetValue(key, out var v) && v is not null && long.TryParse(v.ToString(), out var l) ? l : null;

    public static double? GetDouble(IReadOnlyDictionary<string, object?> row, string key)
        => row.TryGetValue(key, out var v) && v is not null && double.TryParse(v.ToString(), out var d) ? d : null;
}

public sealed class ProviderException : Exception
{
    public ProviderException(string message, Exception? inner = null) : base(message, inner) { }
}
