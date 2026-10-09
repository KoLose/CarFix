using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Data;
using AvaloniaApp.Helpers;
using Domain.Models;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Mechanic.Windows;

public partial class TransferOrderWindow : Window
{
    private readonly int _orderId;
    private Order? _order;

    public TransferOrderWindow() : this(0) { }

    public TransferOrderWindow(int orderId)
    {
        InitializeComponent();
        _orderId = orderId;
        if (orderId > 0)
            _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var orders = new OrderRepository(App.DbContext);
        _order = await orders.GetByIdAsync(_orderId);
        if (_order == null || VariableData.CurrentUser == null) return;

        var serviceIds = _order.OrderServices.Select(os => os.ServiceId);
        var skills = new UserServiceRepository(App.DbContext);
        var candidates = await skills.GetMechanicsWithServicesAsync(serviceIds);
        var users = new UserRepository(App.DbContext);
        var shifts = new ShiftRepository(App.DbContext);

        var available = new List<User>();
        foreach (var mechanic in candidates.Where(m => m.Id != VariableData.CurrentUser.Id))
        {
            var busy = await users.IsBusyAtAsync(mechanic.Id, _order.DateCreated, _order.Id);
            var hasShift = await shifts.HasFreeShiftAsync(mechanic.Id, _order.DateCreated);
            if (!busy && hasShift)
                available.Add(mechanic);
        }

        Grid.ItemsSource = available;
    }

    private async void TransferClick(object? sender, RoutedEventArgs e)
    {
        if (_order == null || Grid.SelectedItem is not User mechanic)
        {
            await DialogHelper.ShowAsync(this, "Выберите механика");
            return;
        }

        _order.MechanicId = mechanic.Id;
        await new OrderRepository(App.DbContext).UpdateAsync(_order);
        Close();
    }
}
