using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaApp.Pages.Manager.Windows;
using Domain.Models;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Manager.Pages;

public partial class Orders : UserControl
{
    public Orders()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var repo = new OrderRepository(App.CreateDbContext());
        Grid.ItemsSource = await repo.GetAllAsync();
    }

    private async void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (Grid.SelectedItem is not Order order) return;
        var win = new OrderDetailsWindow(order.Id);
        var parent = TopLevel.GetTopLevel(this) as Window;
        if (parent != null)
            await win.ShowDialog(parent);
        await LoadAsync();
    }
}
