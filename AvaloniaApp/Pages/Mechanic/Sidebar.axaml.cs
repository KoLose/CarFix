using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Pages.Mechanic.Pages;
using Material.Icons.Avalonia;

namespace AvaloniaApp.Pages.Mechanic;

public partial class Sidebar : UserControl
{
    public Sidebar()
    {
        InitializeComponent();

        if (Content is StackPanel stack)
        {
            foreach (var child in stack.Children)
            {
                if (child is MaterialIcon icon)
                    icon.PointerPressed += OnIconClick;
            }
        }
    }

    private void OnIconClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not MaterialIcon icon || icon.Tag is not string tag)
            return;

        var main = TopLevel.GetTopLevel(this) as MainWindow;
        if (main == null) return;

        main.MainContent.Content = tag switch
        {
            "Orders" => new Orders(),
            "Storage" => new Storage(),
            "Profile" => new AvaloniaApp.Pages.Profile(),
            _ => main.MainContent.Content
        };
    }
}
