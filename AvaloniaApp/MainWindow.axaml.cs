using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContent.Content = new Pages.Auth.LoginUC();
        Sidebar.Content = new Pages.Auth.SidebarUC();
        Header.Content = new Pages.Auth.HeaderUC();
    }
}