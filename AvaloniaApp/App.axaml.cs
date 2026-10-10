using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaApp.Helpers;
using Infastructure.DbContext;

namespace AvaloniaApp;

public partial class App : Application
{
    public static ContextDb DbContext { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DbContext = new ContextDb();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();

            if (!string.IsNullOrEmpty(Program.DbStartupError))
            {
                var error = Program.DbStartupError;
                desktop.MainWindow.Opened += async (_, _) =>
                {
                    await DialogHelper.ShowAsync(
                        desktop.MainWindow,
                        "Не удалось подключиться к базе данных.\n\n" +
                        "1) Запустите PostgreSQL (служба postgresql)\n" +
                        "2) Выполните Infastructure\\Sql\\setup_db.ps1\n" +
                        "3) Проверьте appsettings.json: postgres / 123 / carfixdb\n\n" +
                        error,
                        "Ошибка БД");
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>Новый контекст на операцию (DbContext не потокобезопасен).</summary>
    public static ContextDb CreateDbContext() => new ContextDb();
}
