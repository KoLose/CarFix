using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Helpers;
using Domain.Models;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Manager.Windows;

public partial class AddCarWindow : Window
{
    public AddCarWindow()
    {
        InitializeComponent();
        _ = LoadClientsAsync();
    }

    private async System.Threading.Tasks.Task LoadClientsAsync()
    {
        var repo = new UserRepository(App.CreateDbContext());
        ClientBox.ItemsSource = await repo.GetClientsAsync();
    }

    private async void SaveClick(object? sender, RoutedEventArgs e)
    {
        if (ClientBox.SelectedItem is not User client ||
            string.IsNullOrWhiteSpace(BrandBox.Text) ||
            string.IsNullOrWhiteSpace(ModelBox.Text) ||
            string.IsNullOrWhiteSpace(VinBox.Text) ||
            string.IsNullOrWhiteSpace(PlateBox.Text) ||
            !int.TryParse(YearBox.Text, out var year) ||
            !int.TryParse(MileageBox.Text, out var mileage))
        {
            await DialogHelper.ShowAsync(this, "Проверьте заполнение полей");
            return;
        }

        var repo = new CarRepository(App.CreateDbContext());
        await repo.AddAsync(new Car
        {
            ClientId = client.Id,
            Brand = BrandBox.Text.Trim(),
            Model = ModelBox.Text.Trim(),
            Year = year,
            VIN = VinBox.Text.Trim(),
            PlateNumber = PlateBox.Text.Trim(),
            Mileage = mileage
        });
        Close();
    }
}
