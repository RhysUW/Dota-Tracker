using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Dota_Tracker.API;

/// <summary>
/// Contains methods for querying the steam API
/// 
/// if you want to add new functions, here is the list of methods available in the 
/// steam API: https://steamapi.xpaw.me/IDOTA2Match_570
/// </summary>
public static class DataMethods
{
    private static readonly HttpClient _http = new();
    private static readonly string _apiKey = File.ReadAllText("D:\\Dota\\Dota Tracker\\Dota Tracker\\.env").Trim();

    //changes a users steam id to Account id (32 bits) that Dota match expects
    public static uint SteamId64ToAccountID(ulong steamId64)
    {
        return (uint)(steamId64 - 76561197960265728);
    }

    /// <summary>
    /// gets a list of players with a specific ID
    /// </summary>
    /// <param name="steamId64"> users steam ID</param>
    /// <returns> List of users </returns>
    public static async Task<List<PlayerSummary>?> GetPlayerSummaryAsync(ulong steamId64)
    {
        string url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/" +
                     $"?key={_apiKey}&steamids={steamId64}";

        var result = await _http.GetFromJsonAsync<PlayerSummariesResponse>(url);
        return result?.Response.Players;
    }

    /// <summary>
    /// gets the match id's and start times of the last 20 games the user played
    /// </summary>
    /// <param name="accID"></param>
    /// <returns></returns>
    public static async Task<MatchHistoryResult?> GetPrevious20Matches(uint accID)
    {
        string url = $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/v1/" +
                     $"?key={_apiKey}&account_id={accID}&matches_requested=20";
                    
        var result = await _http.GetFromJsonAsync<MatchHistoryResponse>(url);
        return result?.Result;
    }
}

public record PlayerSummariesResponse(
    [property: JsonPropertyName("response")] PlayerSummariesInner Response);
public record PlayerSummariesInner(
    [property: JsonPropertyName("players")] List<PlayerSummary> Players);
public record PlayerSummary(
    [property: JsonPropertyName("steamid")] string SteamId,
    [property: JsonPropertyName("personaname")] string PersonaName,
    [property: JsonPropertyName("avatarfull")] string AvatarFull
);


public record MatchHistoryResponse(
    [property: JsonPropertyName("result")] MatchHistoryResult Result
);
public record MatchHistoryResult(
    [property: JsonPropertyName("status")] int status,
    [property: JsonPropertyName("num_results")] int numResults,
    [property: JsonPropertyName("matches")] List<MatchSummary> Matches
);

public record MatchSummary(
    [property: JsonPropertyName("match_id")] long MatchId,
    [property: JsonPropertyName("start_time")] long StartTime
);