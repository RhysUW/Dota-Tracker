using Avalonia.Controls;
using Dota_Tracker.ViewModels;

namespace Dota_Tracker.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContextChanged += (_, _) =>
        {
          if(DataContext is MainViewModel vm)
            {
                vm.ErrorOccurred += OnErrorOccurred;
            }  
        };
    }

    private void OnErrorOccurred(string message)
    {
        var errorWindow = new ErrorWindow(message);
        _ = errorWindow.ShowDialog(this);
    }
}