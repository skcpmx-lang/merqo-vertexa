using System.Windows;
using System.Windows.Controls;
using Merqo.Vertexa.Core.Storage;

namespace Merqo.Vertexa.App.Views;

public partial class SettingsPage : Page
{
    private readonly SettingsDatabase _db;

    public SettingsPage()
    {
        InitializeComponent();
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MerqoVertexa", "vertexa.db");
        _db = new SettingsDatabase(path);
        MaskSerials.IsChecked = _db.Get("maskSerials") == "true";
        DarkTheme.IsChecked = _db.Get("theme") != "light";
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        _db.Set("maskSerials", MaskSerials.IsChecked == true ? "true" : "false");
        _db.Set("theme", DarkTheme.IsChecked == true ? "dark" : "light");
        Status.Text = "Settings saved.";
        App.Logger.Info("Settings saved");
    }
}
