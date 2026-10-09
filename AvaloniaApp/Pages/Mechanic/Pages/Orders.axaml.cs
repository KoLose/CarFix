using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaApp.Data;
using AvaloniaApp.Pages.Mechanic.Windows;
using Domain.Models;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Mechanic.Pages;

public partial class Orders : UserControl
{
    public Orders()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        if (VariableData.CurrentUser == null) return;
        var repo = new OrderRepository(App.DbContext);
        Grid.ItemsSource = await repo.GetByMechanicAsync(VariableData.CurrentUser.Id);
    }

    private async void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (Grid.SelectedItem is not Order order) return;
        var win = new MechanicOrderWindow(order.Id);
        var parent = TopLevel.GetTopLevel(this) as Window;
        if (parent != null)
            await win.ShowDialog(parent);
        await LoadAsync();
    }
}
