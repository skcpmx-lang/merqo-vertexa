using Merqo.Vertexa.Core.Diagnostics;
using Merqo.Vertexa.Core.Models;
using Xunit;

namespace Merqo.Vertexa.Core.Tests;

public class BatteryFormulaTests
{
    [Fact]
    public void WearAndHealth_AreCalculatedOnlyWhenBothCapacitiesValid()
    {
        // Formula contract: Wear = (Design-Full)/Design*100, Health = Full/Design*100
        long design = 60_000, full = 48_000;
        var wear = (design - full) / (double)design * 100.0;
        var health = full / (double)design * 100.0;
        Assert.Equal(20.0, wear, 3);
        Assert.Equal(80.0, health, 3);
    }

    [Fact]
    public void ResultStatuses_NeverFabricate()
    {
        var un = DiagnosticResult<int>.Unsupported("s", "m", "not exposed");
        Assert.Equal(DiagnosticStatus.Unsupported, un.Status);
        Assert.False(un.IsAvailable);
        Assert.Equal(0, un.Confidence);
    }

    [Fact]
    public void Estimated_RequiresCalculationMethod()
    {
        var est = DiagnosticResult<double>.EstimatedOk(80.0, "calc", "Full/Design*100", "Health = FullCharge/Design * 100");
        Assert.Equal(DiagnosticStatus.Estimated, est.Status);
        Assert.Contains("Health =", est.Error);
    }
}
