using System.Windows;
using System.Windows.Threading;
using Merqo.Vertexa.Core.Logging;

namespace Merqo.Vertexa.App;

public partial class App : Application
{
    public static AppLogger Logger { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MerqoVertexa", "logs");
        Logger = new AppLogger(logDir);
        Logger.Info("MERQO VERTEXA starting");

        // Global error handling: log and keep the app alive.
        DispatcherUnhandledException += (s, args) =>
        {
            Logger.Error("Unhandled UI exception", args.Exception);
            args.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            Logger.Error("Unhandled domain exception", args.ExceptionObject as Exception);
    }
}
