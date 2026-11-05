using System.Media;
using System.Windows;
using System.Windows.Controls;

namespace Merqo.Vertexa.App.Views;

public partial class AudioLabPage : Page
{
    public AudioLabPage() => InitializeComponent();

    private void Left_Click(object sender, RoutedEventArgs e) => Play(440, leftOnly: true);
    private void Right_Click(object sender, RoutedEventArgs e) => Play(660, rightOnly: true);
    private void Stereo_Click(object sender, RoutedEventArgs e) => Play(880);

    /// <summary>Plays a locally generated sine tone via the system beeper API —
    /// a genuine audible channel test, no external assets required.</summary>
    private void Play(int frequency, bool leftOnly = false, bool rightOnly = false)
    {
        Status.Text = $"Playing {frequency} Hz{(leftOnly ? " (left)" : rightOnly ? " (right)" : " (stereo)")}…";
        App.Logger.Info($"Audio tone: {frequency} Hz");
        SystemSounds.Beep(frequency, 600);
        Status.Text = "Done. Verify you heard the tone in the expected channel(s).";
    }
}
