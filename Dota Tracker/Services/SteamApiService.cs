using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Dota_Tracker.Models;

namespace Dota_Tracker.Services;

/// <summary>
/// Contains methods for querying the steam API and OpenDota API
///
/// if you want to add new functions, here is the list of methods available in the
/// steam API: https://steamapi.xpaw.me/IDOTA2Match_570
/// OpenDota API: https://docs.opendota.com/#section/Introduction
/// </summary>
public class SteamApiService
{
    private readonly HttpClient _http = new();
    private readonly string _apiKey = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, ".env")).Trim();

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
    public async Task<List<PlayerSummary>?> GetPlayerSummaryAsync(ulong steamId64)
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
    public async Task<MatchHistoryResult?> GetPrevious20Matches(ulong accID)
    {
        ulong dotaId = SteamId64ToAccountID(accID);
        string url = $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/v1/" +
                     $"?key={_apiKey}&account_id={dotaId}&matches_requested=20";

        var result = await _http.GetFromJsonAsync<MatchHistoryResponse>(url);
        return result?.Result;
    }

    public async Task<List<MatchInfo>?> GetPrev10MatchesAsync(ulong accId)
    {
        ulong dotaId = SteamId64ToAccountID(accId);
        string url = $"https://api.opendota.com/api/players/{dotaId}/matches?limit=10";
        var result = await _http.GetFromJsonAsync<List<MatchInfo>>(url);
        return result;
    }

    public async Task<List<Heros>?> GetHeros()
    {
        string url = "https://api.opendota.com/api/heroes";
        var results = await _http.GetFromJsonAsync<List<Heros>>(url);
        return results;
    }
}
