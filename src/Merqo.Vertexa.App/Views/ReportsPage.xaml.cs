using System.IO;
using System.Windows;
using System.Windows.Controls;
using Merqo.Vertexa.Core.Diagnostics;
using Merqo.Vertexa.Core.Models;
using Merqo.Vertexa.Core.Reporting;
using Microsoft.Win32;

namespace Merqo.Vertexa.App.Views;

public partial class ReportsPage : Page
{
    private SystemSnapshot? _snapshot;

    public ReportsPage() => InitializeComponent();

    private async void Scan_Click(object sender, RoutedEventArgs e)
    {
        Status.Text = "Running full scan…";
        _snapshot = await new DiagnosticOrchestrator().RunFullScanAsync();
        JsonBtn.IsEnabled = HtmlBtn.IsEnabled = CsvBtn.IsEnabled = true;
        Status.Text = "Scan complete. Choose a format to export.";
    }

    private void Export(string filter, Func<SystemSnapshot, string> render)
    {
        if (_snapshot is null) return;
        var dlg = new SaveFileDialog { Filter = filter, FileName = $"vertexa-report-{DateTime.Now:yyyyMMdd-HHmmss}" };
        if (dlg.ShowDialog() != true) return;
        try
        {
            File.WriteAllText(dlg.FileName, render(_snapshot));
            Status.Text = $"Report exported: {dlg.FileName}";
            App.Logger.Info($"Report exported: {dlg.FileName}");
        }
        catch (Exception ex)
        {
            Status.Text = $"Export failed: {ex.Message}";
            App.Logger.Error("Report export failed", ex);
        }
    }

    private void Json_Click(object sender, RoutedEventArgs e) => Export("JSON|*.json", ReportEngine.ToJson);
    private void Html_Click(object sender, RoutedEventArgs e) => Export("HTML|*.html", ReportEngine.ToHtml);
    private void Csv_Click(object sender, RoutedEventArgs e) => Export("CSV|*.csv", ReportEngine.ToCsv);
}
