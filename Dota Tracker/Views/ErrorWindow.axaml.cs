

using Avalonia.Controls;
using Avalonia.Interactivity;
using Tmds.DBus.Protocol;

namespace Dota_Tracker.Views;

public partial class ErrorWindow : Window
{
    public ErrorWindow()
    {
        InitializeComponent();
    }

    public ErrorWindow(string message) : this()
    {
        MessageText.Text = message;
    }

    private void OnOkClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}