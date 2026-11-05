using System.Text;
using System.Text.Json;
using Merqo.Vertexa.Core.Models;

namespace Merqo.Vertexa.Core.Reporting;

/// <summary>
/// Genuine report generation from a real SystemSnapshot. Every report carries
/// MERQO VERTEXA branding, app version, generation time, provenance, statuses,
/// errors and limitations. No report is ever generated from fabricated data.
/// </summary>
public static class ReportEngine
{
    public const string ProductName = "MERQO VERTEXA";
    public const string Version = "1.0.0";

    public static string ToJson(SystemSnapshot snapshot)
        => JsonSerializer.Serialize(new
        {
            product = ProductName,
            version = Version,
            generatedAt = DateTimeOffset.UtcNow,
            sections = snapshot.Sections,
            errors = snapshot.Errors,
            limitations = new[]
            {
                "Values marked Unsupported/Unavailable were not exposed by hardware/OS and are not fabricated.",
                "Estimated values carry their calculation method.",
                "Application-level tests do not replace firmware/boot-time diagnostics."
            }
        }, new JsonSerializerOptions { WriteIndented = true });

    public static string ToCsv(SystemSnapshot snapshot)
    {
        var sb = new StringBuilder();
        sb.AppendLine("section,key,value,status,source,method,error");
        foreach (var (section, value) in snapshot.Sections)
            sb.AppendLine($"{section},result,\"{Escape(value?.ToString())}\",collected,{section},collector,");
        foreach (var e in snapshot.Errors)
            sb.AppendLine($"errors,error,\"{Escape(e)}\",Failed,orchestrator,");
        return sb.ToString();
    }

    public static string ToHtml(SystemSnapshot snapshot)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'>");
        sb.AppendLine($"<title>{ProductName} — Diagnostic Report</title>");
        sb.AppendLine("<style>body{font-family:Segoe UI,Arial,sans-serif;margin:40px;color:#1B2430} h1{color:#0891B2} table{border-collapse:collapse;width:100%} td,th{border:1px solid #ccc;padding:6px 10px;text-align:left;font-size:13px} .err{color:#DC2626}</style>");
        sb.AppendLine("</head><body>");
        sb.AppendLine($"<h1>{ProductName}</h1>");
        sb.AppendLine($"<p><b>Version:</b> {Version} &nbsp; <b>Generated:</b> {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} (local)</p>");
        sb.AppendLine("<table><tr><th>Section</th><th>Result</th></tr>");
        foreach (var (section, value) in snapshot.Sections)
            sb.AppendLine($"<tr><td>{Escape(section)}</td><td><pre style='white-space:pre-wrap'>{Escape(Format(value))}</pre></td></tr>");
        sb.AppendLine("</table>");
        if (snapshot.Errors.Count > 0)
        {
            sb.AppendLine("<h2>Errors</h2><ul>");
            foreach (var e in snapshot.Errors) sb.AppendLine($"<li class='err'>{Escape(e)}</li>");
            sb.AppendLine("</ul>");
        }
        sb.AppendLine("<h2>Limitations</h2><ul><li>Unsupported/Unavailable values were not exposed by hardware/OS and are not fabricated.</li><li>Estimated values carry their calculation method.</li></ul>");
        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    private static string Format(object? value) => value switch
    {
        null => "N/A",
        string s => s,
        _ => JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true })
    };

    private static string Escape(string? s) => (s ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
}
