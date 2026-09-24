using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContent.Content = new Pages.Auth.RegisterUC();
    }
}