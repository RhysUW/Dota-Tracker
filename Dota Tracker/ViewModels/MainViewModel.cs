using System;
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

    [ObservableProperty]
    public partial string? ErrorMessage { get; set;}

    public Models.PlayerSummary CurrentPlayer {get;}

    [ObservableProperty]
    public partial string PlayerName { get; set;}

    [ObservableProperty]
    public partial Bitmap? PlayerAvatar { get; set; }

    public MainViewModel(PlayerSummary currentPlayer)
    {
        CurrentPlayer = currentPlayer;
        PlayerName = currentPlayer.PersonaName;

        _ = LoadAvatarAsync(currentPlayer.AvatarFull);

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
            ErrorMessage = $"couldnt load the avatar: {e}";
        }
    }
}
