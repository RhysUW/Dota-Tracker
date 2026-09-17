using System.Collections.Generic;
using System.Text.Json.Serialization;

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
