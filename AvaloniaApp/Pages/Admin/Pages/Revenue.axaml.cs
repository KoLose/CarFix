using Avalonia.Controls;
using Avalonia.Interactivity;
using Infastructure.Repositories;

namespace AvaloniaApp.Pages.Admin.Pages;

public partial class Revenue : UserControl
{
    public Revenue()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async void RefreshClick(object? sender, RoutedEventArgs e) => await LoadAsync();

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var payments = new PaymentRepository(App.CreateDbContext());
        var parts = new PartRepository(App.CreateDbContext());
        var totalPayments = await payments.GetTotalAsync();
        var totalParts = await parts.GetTotalPurchaseCostAsync();
        var profit = totalPayments - totalParts;

        PaymentsText.Text = $"Сумма платежей: {totalPayments:N2} ₽";
        PartsText.Text = $"Сумма закупок запчастей: {totalParts:N2} ₽";
        ProfitText.Text = $"Прибыль: {profit:N2} ₽";
    }
}
