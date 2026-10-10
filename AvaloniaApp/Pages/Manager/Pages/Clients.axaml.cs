using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Pages.Manager.Windows;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Manager.Pages;

public partial class Clients : UserControl
{
    public Clients()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var repo = new UserRepository(App.CreateDbContext());
        Grid.ItemsSource = await repo.GetClientsAsync();
    }

    private async void RegisterClick(object? sender, RoutedEventArgs e)
    {
        var win = new RegisterClientWindow();
        var parent = TopLevel.GetTopLevel(this) as Window;
        if (parent != null)
            await win.ShowDialog(parent);
        await LoadAsync();
    }
}
