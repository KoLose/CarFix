using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Helpers;
using Domain.Models;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Manager.Pages;

public partial class Mechanics : UserControl
{
    public Mechanics()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var repo = new UserRepository(App.CreateDbContext());
        Grid.ItemsSource = await repo.GetMechanicsAsync();
    }

    private async void FireClick(object? sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not User mechanic)
        {
            await DialogHelper.ShowAsync(TopLevel.GetTopLevel(this) as Window, "Выберите механика");
            return;
        }

        var users = new UserRepository(App.CreateDbContext());
        if (await users.HasActiveOrdersAsync(mechanic.Id))
        {
            await DialogHelper.ShowAsync(TopLevel.GetTopLevel(this) as Window,
                "Нельзя уволить механика с активными заказами");
            return;
        }

        var shifts = new ShiftRepository(App.CreateDbContext());
        foreach (var shift in await shifts.GetByUserAsync(mechanic.Id))
        {
            shift.UserId = null;
            await shifts.UpdateAsync(shift);
        }

        await users.DeleteAsync(mechanic);
        await LoadAsync();
    }
}
