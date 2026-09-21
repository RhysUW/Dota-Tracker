using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.Messaging;

namespace Dota_Tracker.Models;

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

public record MatchInfo(
    [property: JsonPropertyName("match_id")] long MatchId,
    [property:JsonPropertyName("player_slot")]  int Slot,
    [property:JsonPropertyName("radiant_win")] bool RadWin, 
    [property:JsonPropertyName("duration")] int Duration, 
    [property:JsonPropertyName("game_mode")] int GameMode, 
    [property:JsonPropertyName("lobby_type")] int LobbyType, 
    [property:JsonPropertyName("hero_id")] int Hero, 
    [property:JsonPropertyName("start_time")] long StartTime,
    [property:JsonPropertyName("kills")] int Kills,
    [property:JsonPropertyName("deaths")] int Deaths,
    [property:JsonPropertyName("assists")] int Assists,
    [property:JsonPropertyName("average_rank")] int? AverageRank
)
{
    public bool IsWin => (Slot < 128) == RadWin;
    public string? outcomeFormatted { get; set;}
    public string DurationFormatted => TimeSpan.FromSeconds(Duration).ToString(@"mm\:ss");
    public DateTime PlayedAt => DateTimeOffset.FromUnixTimeSeconds(StartTime).LocalDateTime;
    public string? FormattedHeroName {get; set;}
    public string? HeroIconUrl { get; set; }
}

public record Heros(
    [property: JsonPropertyName("id")] int HeroID,
    [property: JsonPropertyName("name")] string name,
    [property: JsonPropertyName("localized_name")] string HeroNameFormatted,
    [property: JsonPropertyName("primary_attr")] string Attr,
    [property: JsonPropertyName("attack_type")] string AttackType,
    [property: JsonPropertyName("icon")] string Icon
)
{
    public string Shorthand => name.Replace("npc_dota_hero_", "");
    public string? HeroIcon => $"https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/icons/{Shorthand}.png";
}
