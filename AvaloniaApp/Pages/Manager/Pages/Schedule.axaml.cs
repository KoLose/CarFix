using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Helpers;
using Domain.Models;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Manager.Pages;

public partial class Schedule : UserControl
{
    public Schedule()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var shifts = new ShiftRepository(App.DbContext);
        var users = new UserRepository(App.DbContext);
        Grid.ItemsSource = await shifts.GetAllAsync();
        MechanicBox.ItemsSource = await users.GetMechanicsAsync();
    }

    private async void ReplaceClick(object? sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not Shift shift)
        {
            await DialogHelper.ShowAsync(TopLevel.GetTopLevel(this) as Window, "Выберите слот");
            return;
        }

        if (MechanicBox.SelectedItem is not User newMechanic)
        {
            await DialogHelper.ShowAsync(TopLevel.GetTopLevel(this) as Window, "Выберите механика");
            return;
        }

        var users = new UserRepository(App.DbContext);
        if (shift.UserId is int currentId)
        {
            var today = DateTime.Today;
            var target = GetNextDateForDay(shift.DayOfWeek, today);
            if (await users.HasActiveOrdersOnDateAsync(currentId, target))
            {
                await DialogHelper.ShowAsync(TopLevel.GetTopLevel(this) as Window,
                    "Нельзя заменить механика: у текущего есть активные заказы в этот день");
                return;
            }
        }

        shift.UserId = newMechanic.Id;
        var shifts = new ShiftRepository(App.DbContext);
        await shifts.UpdateAsync(shift);
        await LoadAsync();
    }

    private static DateTime GetNextDateForDay(string dayRu, DateTime from)
    {
        var target = dayRu switch
        {
            "Пн" => DayOfWeek.Monday,
            "Вт" => DayOfWeek.Tuesday,
            "Ср" => DayOfWeek.Wednesday,
            "Чт" => DayOfWeek.Thursday,
            "Пт" => DayOfWeek.Friday,
            "Сб" => DayOfWeek.Saturday,
            "Вс" => DayOfWeek.Sunday,
            _ => from.DayOfWeek
        };

        var date = from;
        while (date.DayOfWeek != target)
            date = date.AddDays(1);
        return date;
    }
}
