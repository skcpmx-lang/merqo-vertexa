using System.Windows;
using System.Windows.Controls;
using Merqo.Vertexa.App.Views;

namespace Merqo.Vertexa.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ContentFrame.Navigate(new DashboardPage());
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && b.Tag is string tag)
        {
            Page page = tag switch
            {
                "Dashboard" => new DashboardPage(),
                "Hardware" => new HardwarePage(),
                "Battery" => new BatteryPage(),
                "Storage" => new StoragePage(),
                "Drivers" => new DriversPage(),
                "Reports" => new ReportsPage(),
                "Settings" => new SettingsPage(),
                _ => new DashboardPage(),
            };
            TopBarTitle.Text = tag;
            ContentFrame.Navigate(page);
            App.Logger.Info($"Navigated to {tag}");
        }
    }
}
