using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Helpers;
using Domain.Models;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Manager.Windows;

public partial class RegisterClientWindow : Window
{
    public RegisterClientWindow()
    {
        InitializeComponent();
    }

    private async void SaveClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text) ||
            string.IsNullOrWhiteSpace(LoginBox.Text) ||
            string.IsNullOrWhiteSpace(PasswordBox.Text))
        {
            await DialogHelper.ShowAsync(this, "Заполните все поля");
            return;
        }

        var repo = new UserRepository(App.DbContext);
        await repo.AddAsync(new User
        {
            Name = NameBox.Text.Trim(),
            Login = LoginBox.Text.Trim(),
            Password = PasswordBox.Text,
            RoleId = RoleIds.Client
        });
        Close();
    }
}
