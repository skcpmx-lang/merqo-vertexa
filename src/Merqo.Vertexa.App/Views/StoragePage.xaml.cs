using System.Text;
using System.Windows;
using System.Windows.Controls;
using Merqo.Vertexa.Core.Diagnostics;

namespace Merqo.Vertexa.App.Views;

public partial class StoragePage : Page
{
    public StoragePage() => InitializeComponent();

    private void List_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var sb = new StringBuilder();
            foreach (var d in StorageCollector.Collect())
            {
                sb.AppendLine($"{d.Model} — {d.CapacityBytes / 1024.0 / 1024 / 1024:N0} GB [{d.MediaType}]");
                foreach (var p in d.Partitions)
                    sb.AppendLine($"   {p.DriveLetter}  {p.FileSystem}  {(p.TotalBytes - p.FreeBytes) / 1e9:N1}/{(p.TotalBytes / 1e9):N1} GB used");
            }
            Output.Text = sb.ToString();
        }
        catch (Exception ex) { Output.Text = $"Failed: {ex.Message}"; }
    }

    private async void Bench_Click(object sender, RoutedEventArgs e)
    {
        BenchBtn.IsEnabled = false;
        var dir = Path.Combine(Path.GetTempPath(), "MerqoVertexa", "bench");
        var result = await StorageCollector.SafeWriteBenchmarkAsync(dir, 64L * 1024 * 1024);
        BenchStatus.Text = result.IsAvailable
            ? $"Sequential write: {result.Value:F1} {result.Unit} [{result.Status}]"
            : $"Benchmark not completed: {result.Status} — {result.Error}";
        App.Logger.Info($"Storage benchmark: {BenchStatus.Text}");
        BenchBtn.IsEnabled = true;
    }
}
