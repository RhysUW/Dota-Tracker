using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;


namespace Dota_Tracker.Services;

public class SteamLoginServices
{

    private const string SteamOpenIdUrl = "https://steamcommunity.com/openid/login";
    public async Task<ulong> LoginAsync()
    {
        int port = GetFreePort();
        string returnTo = $"http://localhost:{port}/";

        using var listener = new HttpListener();
        listener.Prefixes.Add(returnTo);
        listener.Start();

        OpenBrowser(BuildAuthUrl(returnTo));

        var contextTask = listener.GetContextAsync();
        var timeoutTask = Task.Delay(TimeSpan.FromMinutes(2));

        if (await Task.WhenAny(contextTask, timeoutTask) == timeoutTask)
        {
            listener.Stop();
            throw new TimeoutException("Steam login timed out - no response after 2 minutes, please try again");
        }

        var context = await contextTask;
        ulong steamId = await VerifyAndExtractSteamIdAsync(context.Request.QueryString);

        return steamId;
    }

    private static int GetFreePort()
    {
        var tcpListener = new TcpListener(IPAddress.Loopback, 0);
        tcpListener.Start();
        int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        tcpListener.Stop();
        return port;
    }

    private static void OpenBrowser(string url)
    {
        Process.Start(new ProcessStartInfo
        {
           FileName = url,
           UseShellExecute = true 
        });
    }

    private static string BuildAuthUrl(string returnUrl)
    {
        var parameters = new Dictionary<string, string>
        {
            ["openid.ns"] = "http://specs.openid.net/auth/2.0",
            ["openid.mode"] = "checkid_setup",
            ["openid.return_to"] = returnUrl,
            ["openid.realm"] = returnUrl,
            ["openid.identity"] = "http://specs.openid.net/auth/2.0/identifier_select",
            ["openid.claimed_id"] = "http://specs.openid.net/auth/2.0/identifier_select"
        };

        string query = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
        return $"{SteamOpenIdUrl}?{query}";
    }

    private static async Task<ulong> VerifyAndExtractSteamIdAsync(NameValueCollection callbackQuery)
    {
        if(callbackQuery["openid.mode"] != "id_res")
        {
            return 0;
        }

        var verifyParams = new List<string>();
        foreach(string? key in callbackQuery.AllKeys)
        {
            if(key is null)
            {
                continue;
            }
            else
            {
                string value = key == "openid.mode" ? "check_authentication" : callbackQuery[key] ?? string.Empty;
                verifyParams.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
            }
        }

        
        using var http = new HttpClient();
        var response = await http.PostAsync(SteamOpenIdUrl,
            new StringContent(string.Join("&", verifyParams), Encoding.UTF8, "application/x-www-form-urlencoded"));

        string responseText = await response.Content.ReadAsStringAsync();
        if (!responseText.Contains("is_valid:true"))
        {
            return 0;
        }

        string? claimedId = callbackQuery["openid.claimed_id"];
        if(claimedId is null)
        {
            return 0;
        }

        string idPart = claimedId[(claimedId.LastIndexOf("/") + 1)..];
        return ulong.TryParse(idPart, out ulong steamId) ? steamId : 0;

    }
}