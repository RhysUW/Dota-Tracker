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
using System.Linq;

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
    public partial List<MatchInfo>? prev10Matches { get; set;}

    [ObservableProperty]
    public partial List<Heros>? Heros {get; set;}

    public MainViewModel(PlayerSummary currentPlayer)
    {
        CurrentPlayer = currentPlayer;
        PlayerName = currentPlayer.PersonaName;

        ulong.TryParse(currentPlayer.SteamId, out AccNum);

        //loading data relevant to the logged in player
        InitializeAzync();
        _ = LoadAvatarAsync(currentPlayer.AvatarFull);
        _ = LoadPrev10Matches(AccNum);

    }

    // Parameterless constructor purely for the XAML designer preview
    public MainViewModel() : this(new PlayerSummary("0", "Design-time User", string.Empty))
    {
    }

    // other functions rely on the data from LoadHeros, so we ensure it initializes first
    private async void InitializeAzync()
    {
        await LoadHeros();
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

    private async Task LoadPrev10Matches(ulong accId)
    {
        if (accId == 0)
        {
            return;
        }
        try
        {
            prev10Matches = await _steamApiService.GetPrev10MatchesAsync(accId);

            if(prev10Matches != null && Heros != null)
            {
                // match is the object bound in the view, uses this to append the formatted hero name, witch we get from another API call
                var heroNames = Heros?.ToDictionary(h => h.HeroID, h => h.HeroNameFormatted) ?? new Dictionary<int, string>();

                // again need to append the heros icon to the match record which is bound in the view.
                var heroSprites = Heros?.ToDictionary(h => h.HeroID, h => h.HeroIcon) ?? new Dictionary<int, string?>();


                foreach(var match in prev10Matches)
                {
                    match.FormattedHeroName = heroNames.TryGetValue(match.Hero, out var name)
                        ? name
                        : $"{match.Hero}";

                    match.HeroIconUrl = heroSprites.TryGetValue(match.Hero, out var sprite)
                        ? sprite
                        : $"{match.Hero}";


                    if (match.IsWin)
                    {
                        match.outcomeFormatted = "Win";
                    }
                    else
                    {
                        match.outcomeFormatted = "Loss";
                    }
                }
            }
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

    private async Task LoadHeros()
    {
        try{
            Heros = await _steamApiService.GetHeros();
        }
        catch(Exception e)
        {
            ReportError($"Couldnt load hero's: {e.Message}");
        }
    }
}
