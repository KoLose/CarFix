using System;
using Avalonia;
using Infastructure.DbContext;

namespace AvaloniaApp;

class Program
{
    public static string? DbStartupError { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        try
        {
            DatabaseBootstrap.EnsureReadyAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            DbStartupError = ex.GetBaseException().Message;
            Console.Error.WriteLine("DB init failed: " + ex);
        }

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("FATAL: " + ex);
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
