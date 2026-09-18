

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dota_Tracker.Models;
using Dota_Tracker.Services;

namespace Dota_Tracker.ViewModels;

public partial class LoginViewModel : ViewModelBase
{

    private readonly SteamLoginServices _steamLoginService = new();
    private readonly SteamApiService _steamApiService = new();

    [ObservableProperty]
    public partial bool IsLoggingIn { get; set;}

    [ObservableProperty]
    public partial string? ErrorMessage { get; set;}

    public event Action<PlayerSummary>? LoginSucceeded;

    [RelayCommand]
    private async Task LoginAsync()
    {
        IsLoggingIn = true;
        ErrorMessage = null;

        try
        {
            ulong steamId = await _steamLoginService.LoginAsync();
            var players = await _steamApiService.GetPlayerSummaryAsync(steamId);
            var player = players?[0];

            if (player is null)
            {
                ErrorMessage = "Logged in, but couldnt fetch steam profile";
                return;
            }

            LoginSucceeded?.Invoke(player);
        }
        catch(Exception e)
        {
            ErrorMessage = $"Login failed: {e.Message}";
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}