using System.Text;
using System.Windows;
using System.Windows.Controls;
using Merqo.Vertexa.Core.Diagnostics;
using Merqo.Vertexa.Core.Models;

namespace Merqo.Vertexa.App.Views;

public partial class BatteryPage : Page
{
    public BatteryPage() => InitializeComponent();

    private void Read_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var b = BatteryCollector.Collect();
            var sb = new StringBuilder();
            sb.AppendLine($"Battery present: {b.IsPresent}");
            void Line<T>(string label, DiagnosticResult<T> r)
                => sb.AppendLine($"{label,-28} {(r.IsAvailable ? $"{r.Value} {r.Unit}" : "—")} [{r.Status}] {r.Source}/{r.Method}{(r.Error is null ? "" : $" — {r.Error}")}");
            Line("Design capacity", b.DesignCapacityMWh);
            Line("Full charge capacity", b.FullChargeCapacityMWh);
            Line("Wear %", b.WearPercent);
            Line("Health %", b.HealthPercent);
            Line("Cycle count", b.CycleCount);
            Line("Charge %", b.ChargePercent);
            Output.Text = sb.ToString();
        }
        catch (Exception ex)
        {
            Output.Text = $"Battery read failed: {ex.Message}";
            App.Logger.Error("Battery read failed", ex);
        }
    }
}
