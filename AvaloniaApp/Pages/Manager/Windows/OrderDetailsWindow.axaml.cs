using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Helpers;
using Domain.Models;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Manager.Windows;

public partial class OrderDetailsWindow : Window
{
    private readonly int _orderId;
    private Order? _order;

    public OrderDetailsWindow() : this(0) { }

    public OrderDetailsWindow(int orderId)
    {
        InitializeComponent();
        _orderId = orderId;
        if (orderId > 0)
            _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var orders = new OrderRepository(App.DbContext);
        var users = new UserRepository(App.DbContext);
        _order = await orders.GetByIdAsync(_orderId);
        if (_order == null) return;

        InfoText.Text =
            $"Заказ #{_order.Id}\n" +
            $"Статус: {_order.Status?.Name}\n" +
            $"Авто: {_order.Automobile?.Brand} {_order.Automobile?.Model}\n" +
            $"Клиент: {_order.Automobile?.Client?.Name}\n" +
            $"Дата: {_order.DateCreated:g}";

        var mechanics = await users.GetMechanicsAsync();
        MechanicBox.ItemsSource = mechanics;
        MechanicBox.SelectedItem = mechanics.FirstOrDefault(m => m.Id == _order.MechanicId);
    }

    private async void AssignClick(object? sender, RoutedEventArgs e)
    {
        if (_order == null || MechanicBox.SelectedItem is not User mechanic) return;

        var users = new UserRepository(App.DbContext);
        var busy = await users.IsBusyAtAsync(mechanic.Id, _order.DateCreated, _order.Id);
        if (busy)
        {
            ErrorText.Text = "Механик уже занят заказом в это время.";
            return;
        }

        _order.MechanicId = mechanic.Id;
        if (_order.StatusId == StatusIds.New)
            _order.StatusId = StatusIds.InProgress;

        var orders = new OrderRepository(App.DbContext);
        await orders.UpdateAsync(_order);
        ErrorText.Text = string.Empty;
        await DialogHelper.ShowAsync(this, "Механик назначен");
        Close();
    }

    private async void ReviewClick(object? sender, RoutedEventArgs e)
    {
        if (_order == null) return;
        if (_order.StatusId != StatusIds.Completed)
        {
            await DialogHelper.ShowAsync(this, "Отзыв можно оставить только после завершения заказа");
            return;
        }

        var comments = new CommentRepository(App.DbContext);
        if (await comments.HasFinalClientReviewAsync(_order.Id))
        {
            await DialogHelper.ShowAsync(this, "Финальный отзыв уже оставлен");
            return;
        }

        if (string.IsNullOrWhiteSpace(ReviewBox.Text))
        {
            await DialogHelper.ShowAsync(this, "Введите текст отзыва");
            return;
        }

        await comments.AddAsync(new Comment
        {
            OrderId = _order.Id,
            Text = ReviewBox.Text.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsFinalClientReview = true
        });
        await DialogHelper.ShowAsync(this, "Отзыв сохранён");
    }
}
