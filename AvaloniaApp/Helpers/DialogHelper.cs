using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AvaloniaApp.Helpers;

public static class DialogHelper
{
    public static async Task ShowAsync(Window? owner, string message, string title = "Сообщение")
    {
        var window = new Window
        {
            Title = title,
            Width = 420,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Background = Brush.Parse("#17172c"),
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
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Classes = { "Button-UI" }
                    }
                }
            }
        };

        if (window.Content is StackPanel panel && panel.Children[1] is Button ok)
            ok.Click += (_, _) => window.Close();

        if (owner != null)
            await window.ShowDialog(owner);
        else
            window.Show();
    }
}
