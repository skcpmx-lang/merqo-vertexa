using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Merqo.Vertexa.Core.Diagnostics;
using Merqo.Vertexa.Core.Models;

namespace Merqo.Vertexa.App.Views;

public partial class DashboardPage : Page
{
    private SystemSnapshot? _snapshot;

    public DashboardPage() => InitializeComponent();

    private async void Scan_Click(object sender, RoutedEventArgs e)
    {
        ScanBtn.IsEnabled = false;
        Cards.Children.Clear();
        var orch = new DiagnosticOrchestrator();
        var progress = new Progress<DiagnosticProgress>(p =>
        {
            ScanProgress.Value = p.Fraction;
            ScanStatus.Text = $"Scanning {p.Module} ({p.Completed}/{p.Total})…";
        });
        try
        {
            _snapshot = await orch.RunFullScanAsync(progress, CancellationToken.None);
            ScanStatus.Text = _snapshot.Errors.Count == 0
                ? "Scan complete."
                : $"Scan complete with {_snapshot.Errors.Count} module error(s) — see logs.";
            RenderCards();
        }
        catch (Exception ex)
        {
            ScanStatus.Text = $"Scan failed: {ex.Message}";
            App.Logger.Error("Full scan failed", ex);
        }
        finally
        {
            ScanBtn.IsEnabled = true;
        }
    }

    private void RenderCards()
    {
        if (_snapshot is null) return;
        foreach (var (name, value) in _snapshot.Sections)
        {
            var card = new Border { Style = (Style)FindResource("Card"), Width = 340 };
            var panel = new StackPanel();
            panel.Children.Add(new TextBlock { Text = name, FontWeight = FontWeights.SemiBold, Foreground = (Brush)FindResource("AccentBrush") });
            panel.Children.Add(new TextBlock
            {
                Text = value is string s ? s : System.Text.Json.JsonSerializer.Serialize(value),
                Foreground = (Brush)FindResource("TextBrush"),
                TextWrapping = TextWrapping.Wrap,
                FontFamily = new FontFamily("Cascadia Code, Consolas, monospace"),
                FontSize = 11,
                MaxHeight = 160,
            });
            card.Child = panel;
            Cards.Children.Add(card);
        }
    }
}
