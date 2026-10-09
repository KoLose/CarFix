using Avalonia.Controls;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Admin.Pages;

public partial class Storage : UserControl
{
    public Storage()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var repo = new PartRepository(App.DbContext);
        Grid.ItemsSource = await repo.GetAllAsync();
    }
}
