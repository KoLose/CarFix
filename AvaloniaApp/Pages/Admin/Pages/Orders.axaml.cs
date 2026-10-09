using Avalonia.Controls;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Admin.Pages;

public partial class Orders : UserControl
{
    public Orders()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var repo = new OrderRepository(App.DbContext);
        Grid.ItemsSource = await repo.GetAllAsync();
    }
}
