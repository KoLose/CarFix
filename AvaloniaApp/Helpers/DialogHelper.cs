using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AvaloniaApp.Helpers;

public static class DialogHelper
{
    public static async Task ShowAsync(Window? owner, string message, string title = "Сообщение")
    {
        var ok = new Button
        {
            Content = "OK",
            HorizontalAlignment = HorizontalAlignment.Right,
            Classes = { "Button-UI" }
        };

        var window = new Window
        {
            Title = title,
            Width = 480,
            Height = 220,
            WindowStartupLocation = owner != null
                ? WindowStartupLocation.CenterOwner
                : WindowStartupLocation.CenterScreen,
            Background = new SolidColorBrush(Color.Parse("#17172c")),
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = Brushes.White
                    },
                    ok
                }
            }
        };

        ok.Click += (_, _) => window.Close();

        if (owner != null)
            await window.ShowDialog(owner);
        else
        {
            window.Show();
            await Task.CompletedTask;
        }
    }
}
