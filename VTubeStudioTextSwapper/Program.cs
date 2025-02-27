using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace VTubeStudioTextSwapper;

class Program
{
    private static IConfiguration? _configuration;
    private static TwitchClient? _client;
    private static TwitchAPI? _api;
    private static string _channelName = string.Empty;
    private static string _botUsername = string.Empty;
    private static string _clientId = string.Empty;
    private static string _clientSecret = string.Empty;
    private static string _accessToken = string.Empty;
    private static TextToPngRenderer? _textToPngRenderer;

    static async Task Main(string[] args)
    {
        
        // Load configuration
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _channelName = _configuration["TwitchSettings:ChannelName"];
        _botUsername = _configuration["TwitchSettings:BotUsername"];
        _clientId = _configuration["TwitchSettings:BotClientId"];
        _clientSecret = _configuration["TwitchSettings:BotClientSecret"];
        
        string? outputPath = _configuration["TextSettings:OutputPath"];
        string? fontName = _configuration["TextSettings:FontName"];
        int fontSize = int.Parse(_configuration["TextSettings:FontSize"]);
        int padding = int.Parse(_configuration["TextSettings:FontSize"]);
        _textToPngRenderer = new TextToPngRenderer(outputPath, fontName, fontSize, padding);
        
        // Get OAuth token dynamically
        _accessToken = await GetOAuthTokenAsync();
        
        // Initialize Twitch API
        _api = new TwitchAPI
        {
            Settings =
            {
                ClientId = _clientId,
                AccessToken = _accessToken
            }
        };

        if (string.IsNullOrEmpty(_accessToken))
        {
            Console.WriteLine("Failed to retrieve OAuth token.");
            return;
        }

        ConnectionCredentials credentials = new(_botUsername, _accessToken);
        _client = new TwitchClient();
        _client.Initialize(credentials, _channelName);
        Console.WriteLine($"Initialized Twitch client for {_channelName}");

        // Event Handlers
        _client.OnConnected += Client_OnConnected;
        _client.OnJoinedChannel += Client_OnJoinedChannel;
        _client.OnMessageReceived += Client_OnMessageReceived;
        _client.OnError += Client_OnError;
        _client.OnMessageReceived += Client_OnMessageReceived;

        _client.Connect();
       
        // Keep application running in background
        await Task.Delay(-1);
    }

    private static void Client_OnConnected(object? sender, OnConnectedArgs e)
    {
        Console.WriteLine($"Connected to Twitch as {e.BotUsername}");
        _client?.JoinChannel(_channelName);
        Console.WriteLine($"Attempting to join {_channelName}'s channel...");
    }
    
    private static void Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e) => 
        Console.WriteLine($"Bot has joined the channel: {e.Channel}");
    
    private static void Client_OnError(object? sender, OnErrorEventArgs e) => 
        Console.WriteLine($"Error: {e.Exception.Message}");

    private static void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        Console.WriteLine($"Message received from {e.ChatMessage.Username}: {e.ChatMessage.Message}");
        
        if (e.ChatMessage.Username.ToLower() != _channelName.ToLower())
            return;

        string[] messageParts = e.ChatMessage.Message.Split(' ');

        if (!messageParts.Any())
            return;

        string? command = messageParts.FirstOrDefault();
        string? username = messageParts.Skip(1).FirstOrDefault();

        if (command == null || username == null)
        {
            Console.WriteLine("Invalid command");
            return;
        }

        switch (command)
        {
            case "!tail":
                HandleTailCommand(username);
                break;
        }
    }

    private static void HandleTailCommand(string username)
    {
        Console.WriteLine($"Tail command triggered for: {username}");
        _textToPngRenderer?.RenderTextToPng(username);
    }
    
    private static async Task<string> GetOAuthTokenAsync()
    {
        const string tokenFilePath = "tokens.json";

        // Check if there's a saved token
        if (File.Exists(tokenFilePath))
        {
            string tokenData = await File.ReadAllTextAsync(tokenFilePath);
            var tokenJson = JsonDocument.Parse(tokenData).RootElement;

            if (tokenJson.TryGetProperty("access_token", out JsonElement accessToken))
            {
                // Validate token before using it
                if (await ValidateTokenAsync(accessToken.GetString()!))
                {
                    Console.WriteLine("✅ Using saved OAuth token.");
                    return accessToken.GetString()!;
                }
            }

            if (tokenJson.TryGetProperty("refresh_token", out JsonElement refreshToken))
            {
                Console.WriteLine("🔄 Refreshing expired token...");
                return await RefreshOAuthTokenAsync(refreshToken.GetString()!);
            }
        }

        Console.WriteLine("⚠️ No valid token found. You need to authorize the bot.");
        return await GenerateUserOAuthTokenAsync();
    }
    
    private static async Task<bool> ValidateTokenAsync(string accessToken)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        HttpResponseMessage response = await client.GetAsync("https://id.twitch.tv/oauth2/validate");

        return response.IsSuccessStatusCode;
    }
    
    private static async Task<string> RefreshOAuthTokenAsync(string refreshToken)
    {
        using HttpClient client = new();
        var requestUri = $"https://id.twitch.tv/oauth2/token" +
                         $"?client_id={_clientId}" +
                         $"&client_secret={_clientSecret}" +
                         $"&grant_type=refresh_token" +
                         $"&refresh_token={refreshToken}";

        HttpResponseMessage response = await client.PostAsync(requestUri, null);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Failed to refresh token. You'll need to reauthorize.");
            return await GenerateUserOAuthTokenAsync();
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();
        SaveTokenToFile(jsonResponse);

        using JsonDocument json = JsonDocument.Parse(jsonResponse);
        return json.RootElement.GetProperty("access_token").GetString()!;
    }
    
    private static async Task<string> GenerateUserOAuthTokenAsync()
    {
        Console.WriteLine("\nOpen the following URL in a browser and authorize the bot:");
        string authUrl = $"https://id.twitch.tv/oauth2/authorize" +
                         $"?client_id={_clientId}" +
                         $"&redirect_uri=http://localhost" +
                         $"&response_type=code" +
                         $"&scope=chat:read+chat:edit";
    
        Console.WriteLine(authUrl);
        Console.Write("\n🔑 Enter the authorization code from Twitch: ");
        string authCode = Console.ReadLine() ?? "";

        using HttpClient client = new();
        var requestUri = $"https://id.twitch.tv/oauth2/token" +
                         $"?client_id={_clientId}" +
                         $"&client_secret={_clientSecret}" +
                         $"&code={authCode}" +
                         $"&grant_type=authorization_code" +
                         $"&redirect_uri=http://localhost";

        HttpResponseMessage response = await client.PostAsync(requestUri, null);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Error retrieving OAuth token.");
            return string.Empty;
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();
        SaveTokenToFile(jsonResponse);

        using JsonDocument json = JsonDocument.Parse(jsonResponse);
        return json.RootElement.GetProperty("access_token").GetString()!;
    }

    private static void SaveTokenToFile(string jsonResponse)
    {
        File.WriteAllText("tokens.json", jsonResponse);
        Console.WriteLine("Token saved successfully.");
    }
}