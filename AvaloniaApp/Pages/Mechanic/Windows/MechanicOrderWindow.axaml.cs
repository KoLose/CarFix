using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Data;
using AvaloniaApp.Helpers;
using Domain.Models;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Mechanic.Windows;

public partial class MechanicOrderWindow : Window
{
    private readonly int _orderId;
    private Order? _order;
    private bool _canTake;

    public MechanicOrderWindow() : this(0) { }

    public MechanicOrderWindow(int orderId)
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

        InfoText.Text =
            $"Заказ #{_order.Id}\n" +
            $"Статус: {_order.Status?.Name}\n" +
            $"Механик: {_order.Mechanic?.Name}\n" +
            $"Авто: {_order.Automobile?.Brand} {_order.Automobile?.Model}";

        ServicesGrid.ItemsSource = _order.OrderServices.ToList();

        var serviceIds = _order.OrderServices.Select(os => os.ServiceId);
        var skills = new UserServiceRepository(App.DbContext);
        var hasSkills = await skills.HasAllServicesAsync(VariableData.CurrentUser.Id, serviceIds);
        var shifts = new ShiftRepository(App.DbContext);
        var hasShift = await shifts.HasFreeShiftAsync(VariableData.CurrentUser.Id, _order.DateCreated);

        _canTake = hasSkills && hasShift;
        TakeBtn.IsEnabled = _canTake;
        StatusText.Text = !_canTake
            ? (!hasSkills
                ? "Кнопка «Забрать себе» недоступна: нет нужных навыков (UserService)."
                : "Кнопка «Забрать себе» недоступна: нет свободной смены в расписании на эту дату.")
            : "Можно забрать заказ себе.";
    }

    private async void TakeClick(object? sender, RoutedEventArgs e)
    {
        if (_order == null || VariableData.CurrentUser == null || !_canTake) return;

        var users = new UserRepository(App.DbContext);
        if (await users.IsBusyAtAsync(VariableData.CurrentUser.Id, _order.DateCreated, _order.Id))
        {
            await DialogHelper.ShowAsync(this, "У вас уже есть заказ на это время");
            return;
        }

        _order.MechanicId = VariableData.CurrentUser.Id;
        _order.StatusId = StatusIds.InProgress;
        await new OrderRepository(App.DbContext).UpdateAsync(_order);
        await DialogHelper.ShowAsync(this, "Заказ назначен вам");
        await LoadAsync();
    }

    private async void TransferClick(object? sender, RoutedEventArgs e)
    {
        if (_order == null) return;
        var win = new TransferOrderWindow(_order.Id);
        await win.ShowDialog(this);
        await LoadAsync();
    }

    private async void CompleteServiceClick(object? sender, RoutedEventArgs e)
    {
        if (_order == null) return;
        if (ServicesGrid.SelectedItem is not OrderService orderService)
        {
            await DialogHelper.ShowAsync(this, "Выберите услугу");
            return;
        }

        if (orderService.IsCompleted)
        {
            await DialogHelper.ShowAsync(this, "Услуга уже завершена");
            return;
        }

        var parts = new PartRepository(App.DbContext);
        foreach (var op in _order.OrderParts)
        {
            if (!await parts.HasEnoughAsync(op.PartId, op.Quantity))
            {
                await DialogHelper.ShowAsync(this,
                    $"Недостаточно запчастей на складе: {op.Part?.Name ?? op.PartId.ToString()}");
                return;
            }
        }

        var orders = new OrderRepository(App.DbContext);
        await orders.CompleteServiceAsync(orderService);

        _order = await orders.GetByIdAsync(_orderId);
        if (_order != null && _order.OrderServices.All(s => s.IsCompleted))
        {
            foreach (var op in _order.OrderParts)
            {
                var part = await parts.GetByIdAsync(op.PartId);
                if (part == null) continue;
                part.Quantity -= op.Quantity;
                await parts.UpdateAsync(part);
            }

            _order.StatusId = StatusIds.Completed;
            _order.DateFinished = DateTime.UtcNow;
            await orders.UpdateAsync(_order);
        }

        await LoadAsync();
    }

    private async void CommentClick(object? sender, RoutedEventArgs e)
    {
        if (_order == null || string.IsNullOrWhiteSpace(CommentBox.Text)) return;
        await new CommentRepository(App.DbContext).AddAsync(new Comment
        {
            OrderId = _order.Id,
            Text = CommentBox.Text.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsFinalClientReview = false
        });
        CommentBox.Text = string.Empty;
        await DialogHelper.ShowAsync(this, "Комментарий добавлен");
    }
}
