using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Merqo.Vertexa.App.Views;

public partial class KeyboardLabPage : Page
{
    private readonly HashSet<string> _pressed = new(StringComparer.OrdinalIgnoreCase);

    public KeyboardLabPage()
    {
        InitializeComponent();
        Loaded += (_, _) => CaptureArea.Focus();
        CaptureArea.MouseDown += (_, _) => CaptureArea.Focus();
        PreviewKeyDown += OnKey;
    }

    private void OnKey(object sender, KeyEventArgs e)
    {
        var name = e.Key.ToString();
        _pressed.Add(name);
        Hint.Text = "";
        LastKey.Text = $"Last key: {name}";
        Passed.Text = $"Detected keys ({_pressed.Count}): {string.Join(", ", _pressed.OrderBy(k => k))}";
        e.Handled = true;
    }
}
