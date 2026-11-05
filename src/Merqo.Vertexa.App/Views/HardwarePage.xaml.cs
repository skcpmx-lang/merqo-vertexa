using System.Text;
using System.Windows;
using System.Windows.Controls;
using Merqo.Vertexa.Core.Diagnostics;

namespace Merqo.Vertexa.App.Views;

public partial class HardwarePage : Page
{
    public HardwarePage() => InitializeComponent();

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        Summary.Text = "Collecting…";
        try
        {
            var snap = await new DiagnosticOrchestrator().RunFullScanAsync();
            var sb = new StringBuilder();
            foreach (var (name, value) in snap.Sections)
            {
                sb.AppendLine($"=== {name} ===");
                sb.AppendLine(value is string s ? s : System.Text.Json.JsonSerializer.Serialize(value, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                sb.AppendLine();
            }
            Details.Text = sb.ToString();
            Summary.Text = $"Collected at {snap.CapturedAt:HH:mm:ss} — {snap.Sections.Count} sections, {snap.Errors.Count} errors.";
        }
        catch (Exception ex)
        {
            Summary.Text = $"Collection failed: {ex.Message}";
            App.Logger.Error("Hardware refresh failed", ex);
        }
    }
}
