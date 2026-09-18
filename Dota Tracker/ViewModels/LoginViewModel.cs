

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Dota_Tracker.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string SteamId {get; set;} = string.Empty;

    public event Action? LoginSucceeded;

    [RelayCommand]
    private void Login()
    {
        if (string.IsNullOrWhiteSpace(SteamId)){
            return;
        }

        //later: send steam login http request

        LoginSucceeded?.Invoke();
    }
}