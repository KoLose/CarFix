using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Data;
using AvaloniaApp.Helpers;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Auth;

public partial class LoginUC : UserControl
{
    public LoginUC()
    {
        InitializeComponent();
    }

    private async void LoginBtn(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(LoginBox.Text) || string.IsNullOrEmpty(PasswordBox.Text))
            return;

        var owner = TopLevel.GetTopLevel(this) as Window;

        try
        {
            await using var db = App.CreateDbContext();
            var userRepository = new UserRepository(db);
            var user = await userRepository.GetByCredentialsAsync(LoginBox.Text, PasswordBox.Text);

            if (user == null)
            {
                await DialogHelper.ShowAsync(owner, "Неверный логин или пароль");
                return;
            }

            VariableData.CurrentUser = user;

            if (owner is MainWindow main)
            {
                main.Sidebar.Content = user.RoleId switch
                {
                    1 => new Admin.Sidebar(),
                    2 => new Mechanic.Sidebar(),
                    3 => new Manager.Sidebar(),
                    _ => main.Sidebar.Content
                };

                main.Header.Content = new AvaloniaApp.Pages.HeaderUC();

                main.MainContent.Content = user.RoleId switch
                {
                    1 => new Admin.Pages.Orders(),
                    2 => new Mechanic.Pages.Orders(),
                    3 => new Manager.Pages.Orders(),
                    _ => main.MainContent.Content
                };
            }
        }
        catch (System.Exception ex)
        {
            await DialogHelper.ShowAsync(owner, "Ошибка входа / БД:\n" + ex.GetBaseException().Message);
        }
    }
}

