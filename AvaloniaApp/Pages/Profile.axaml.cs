using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Data;
using AvaloniaApp.Helpers;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages;

public partial class Profile : UserControl
{
    public Profile()
    {
        InitializeComponent();
        if (VariableData.CurrentUser == null) return;
        NameBox.Text = VariableData.CurrentUser.Name;
        LoginBox.Text = VariableData.CurrentUser.Login;
        PasswordBox.Text = VariableData.CurrentUser.Password;
    }

    private async void SaveClick(object? sender, RoutedEventArgs e)
    {
        if (VariableData.CurrentUser == null) return;
        if (string.IsNullOrWhiteSpace(NameBox.Text) ||
            string.IsNullOrWhiteSpace(LoginBox.Text) ||
            string.IsNullOrWhiteSpace(PasswordBox.Text))
        {
            await DialogHelper.ShowAsync(TopLevel.GetTopLevel(this) as Window, "Заполните все поля");
            return;
        }

        VariableData.CurrentUser.Name = NameBox.Text.Trim();
        VariableData.CurrentUser.Login = LoginBox.Text.Trim();
        VariableData.CurrentUser.Password = PasswordBox.Text;

        var repo = new UserRepository(App.DbContext);
        await repo.UpdateAsync(VariableData.CurrentUser);
        StatusText.Text = "Данные сохранены";
    }
}
