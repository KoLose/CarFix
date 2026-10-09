using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Infastructure.DbContext;

namespace AvaloniaApp;

public partial class App : Application
{
    public static ContextDb DbContext { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        DbContext = new ContextDb();

        try
        {
            await DbInitializer.InitializeAsync(DbContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DB init failed: {ex.Message}");
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
