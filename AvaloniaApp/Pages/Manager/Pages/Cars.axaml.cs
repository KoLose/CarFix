using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Pages.Manager.Windows;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Manager.Pages;

public partial class Cars : UserControl
{
    public Cars()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var repo = new CarRepository(App.DbContext);
        Grid.ItemsSource = await repo.GetAllAsync();
    }

    private async void AddClick(object? sender, RoutedEventArgs e)
    {
        var win = new AddCarWindow();
        var parent = TopLevel.GetTopLevel(this) as Window;
        if (parent != null)
            await win.ShowDialog(parent);
        await LoadAsync();
    }
}
