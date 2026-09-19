using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Dota_Tracker.Models;
using Dota_Tracker.Services;

namespace Dota_Tracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly SteamApiService _steamApiService = new();

    public Models.PlayerSummary CurrentPlayer {get;}

    //steam accID used to get the dota accId
    private ulong AccNum = 0;

    public event Action<string>? ErrorOccurred; 
    
    [ObservableProperty]
    public partial string? ErrorMessage { get; set;}

    [ObservableProperty]
    public partial string PlayerName { get; set;}

    [ObservableProperty]
    public partial Bitmap? PlayerAvatar { get; set; }

    [ObservableProperty]
    public partial List<MatchInfo>? prev20Matches { get; set;}

    public MainViewModel(PlayerSummary currentPlayer)
    {
        CurrentPlayer = currentPlayer;
        PlayerName = currentPlayer.PersonaName;

        ulong.TryParse(currentPlayer.SteamId, out AccNum);

        //loading data relevant to the logged in player
        _ = LoadAvatarAsync(currentPlayer.AvatarFull);
        _ = LoadPrev20Matches(AccNum);

    }

    // Parameterless constructor purely for the XAML designer preview
    public MainViewModel() : this(new PlayerSummary("0", "Design-time User", string.Empty))
    {
    }

    private async Task LoadAvatarAsync(string avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(avatarUrl))
        {
            return;
        }

        try
        {
            using var http = new HttpClient();
            byte[] bytes = await http.GetByteArrayAsync(avatarUrl);
            using var stream = new MemoryStream(bytes);
            PlayerAvatar = new Bitmap(stream);
        }
        catch (Exception e)
        {
            ReportError($"couldnt load the avatar: {e}");
        }
    }

    private async Task LoadPrev20Matches(ulong accId)
    {
        if (accId == 0)
        {
            return;
        }
        try
        {
        using var http = new HttpClient();
        prev20Matches = await _steamApiService.GetPrev20MatchesAsync(accId);
        }
        catch (Exception e)
        {
            ReportError($"couldn't load match history: {e.Message}");
        }

    }

    private void ReportError(string message)
    {
        ErrorMessage = message;
        ErrorOccurred?.Invoke(message);
    }
}
