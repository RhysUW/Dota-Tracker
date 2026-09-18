using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Dota_Tracker.ViewModels;
using Dota_Tracker.Views;

namespace Dota_Tracker;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var LoginViewModel= new LoginViewModel();
            var LoginWindow = new LoginWindow {DataContext = LoginViewModel};

            LoginViewModel.LoginSucceeded += player =>
            {
              var mainWindow = new MainWindow { DataContext = new MainViewModel(player) };  
              desktop.MainWindow = mainWindow;
              mainWindow.Show();
              LoginWindow.Close();

            };

            desktop.MainWindow = LoginWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}