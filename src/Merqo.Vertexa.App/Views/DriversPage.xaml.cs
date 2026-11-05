using System.Text;
using System.Windows;
using System.Windows.Controls;
using Merqo.Vertexa.Core.Diagnostics;

namespace Merqo.Vertexa.App.Views;

public partial class DriversPage : Page
{
    public DriversPage() => InitializeComponent();

    private void Scan_Click(object sender, RoutedEventArgs e)
    {
        var issues = DriverCollector.CollectIssues();
        if (issues.Count == 0) { Output.Text = "No devices with problem codes detected."; return; }
        var sb = new StringBuilder();
        foreach (var i in issues)
            sb.AppendLine($"{i.DeviceName} — {DriverCollector.DescribeProblem(i.ProblemCode)} ({i.HardwareId})");
        Output.Text = sb.ToString();
    }
}
