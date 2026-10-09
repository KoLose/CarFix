using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Data;
using AvaloniaApp.Helpers;
using Domain.Models;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Mechanic.Pages;

public partial class Storage : UserControl
{
    public Storage()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        if (VariableData.CurrentUser == null) return;
        var repo = new PartRequestRepository(App.DbContext);
        Grid.ItemsSource = await repo.GetByUserAsync(VariableData.CurrentUser.Id);
    }

    private async void SendClick(object? sender, RoutedEventArgs e)
    {
        if (VariableData.CurrentUser == null) return;
        if (string.IsNullOrWhiteSpace(NameBox.Text) || !int.TryParse(QtyBox.Text, out var qty) || qty <= 0)
        {
            await DialogHelper.ShowAsync(TopLevel.GetTopLevel(this) as Window, "Укажите название и количество");
            return;
        }

        var repo = new PartRequestRepository(App.DbContext);
        await repo.AddAsync(new PartRequest
        {
            ItemName = NameBox.Text.Trim(),
            Quantity = qty,
            Note = NoteBox.Text?.Trim() ?? string.Empty,
            UserId = VariableData.CurrentUser.Id,
            CreatedAt = DateTime.UtcNow
        });

        NameBox.Text = string.Empty;
        QtyBox.Text = string.Empty;
        NoteBox.Text = string.Empty;
        await LoadAsync();
    }
}
