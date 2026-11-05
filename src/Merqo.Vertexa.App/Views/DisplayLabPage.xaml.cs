using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Merqo.Vertexa.App.Views;

public partial class DisplayLabPage : Page
{
    private static readonly (string Name, Color Color)[] Patterns =
    {
        ("White", Colors.White), ("Black", Colors.Black), ("Red", Colors.Red),
        ("Green", Colors.Lime), ("Blue", Colors.Blue), ("Yellow", Colors.Yellow),
        ("Cyan", Colors.Cyan), ("Magenta", Colors.Magenta), ("Gray", Colors.Gray),
    };

    public DisplayLabPage()
    {
        InitializeComponent();
        foreach (var (name, color) in Patterns)
        {
            var btn = new Button { Content = name, Padding = new Thickness(14, 8), Margin = new Thickness(4), Tag = color };
            btn.Click += (_, _) => ShowFullscreen(color);
            Patterns.Children.Add(btn);
        }
        var grad = new Button { Content = "Gradient", Padding = new Thickness(14, 8), Margin = new Thickness(4) };
        grad.Click += (_, _) => ShowGradient();
        Patterns.Children.Add(grad);
    }

    private void ShowFullscreen(Color color)
    {
        var w = new Window
        {
            WindowStyle = WindowStyle.None,
            WindowState = WindowState.Maximized,
            Background = new SolidColorBrush(color),
            Title = "MERQO VERTEXA Display Test — ESC to exit",
            Topmost = true,
        };
        w.KeyDown += (s, e) => { if (e.Key == Key.Escape) w.Close(); };
        w.MouseDown += (s, e) => w.Close();
        w.Show();
    }

    private void ShowGradient()
    {
        var brush = new LinearGradientBrush(Colors.Black, Colors.White, 0);
        var w = new Window
        {
            WindowStyle = WindowStyle.None,
            WindowState = WindowState.Maximized,
            Background = brush,
            Title = "MERQO VERTEXA Gradient Test — ESC to exit",
            Topmost = true,
        };
        w.KeyDown += (s, e) => { if (e.Key == Key.Escape) w.Close(); };
        w.Show();
    }
}
