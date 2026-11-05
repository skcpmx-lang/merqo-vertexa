using Merqo.Vertexa.Core.Models;
using Merqo.Vertexa.Core.Reporting;
using Xunit;

namespace Merqo.Vertexa.Core.Tests;

public class ReportEngineTests
{
    private static SystemSnapshot SampleSnapshot() => new()
    {
        Sections = { ["CPU"] = "Intel i7", ["Battery"] = "ok" },
        Errors = { "GPU: sample error" },
    };

    [Fact]
    public void JsonReport_ContainsBrandingVersionAndLimitations()
    {
        var json = ReportEngine.ToJson(SampleSnapshot());
        Assert.Contains("MERQO VERTEXA", json);
        Assert.Contains("\"version\"", json);
        Assert.Contains("not fabricated", json);
    }

    [Fact]
    public void HtmlReport_ContainsSectionsAndErrors()
    {
        var html = ReportEngine.ToHtml(SampleSnapshot());
        Assert.Contains("MERQO VERTEXA", html);
        Assert.Contains("CPU", html);
        Assert.Contains("sample error", html);
    }

    [Fact]
    public void CsvReport_HasHeaderAndRows()
    {
        var csv = ReportEngine.ToCsv(SampleSnapshot());
        Assert.StartsWith("section,key,value", csv);
        Assert.Contains("CPU", csv);
    }
}
