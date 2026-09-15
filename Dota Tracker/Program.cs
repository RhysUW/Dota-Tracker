using Dota_Tracker.API;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/test", async () =>
{
    ulong testAccID = 76561198070196960;
    uint dotaAccID = DataMethods.SteamId64ToAccountID(76561198070196960);

    var player = await DataMethods.GetPlayerSummaryAsync(testAccID);

    var matches = await DataMethods.GetPrevious20Matches(dotaAccID);
    
    return matches;
});

app.MapGet("/", () => "Hello World!");

app.Run();
