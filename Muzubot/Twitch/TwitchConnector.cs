using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Muzubot.Twitch;

public class TwitchConnector
{
    public TwitchConnector(Configuration config, ILogger<TwitchConnector> logger)
    {
        _logger = logger;

        ConnectionCredentials credentials = new ConnectionCredentials(config.TwitchUsername, config.TwitchOauth);
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        WebSocketClient customClient = new WebSocketClient(clientOptions);
        _client = new TwitchClient(customClient);
        _client.Initialize(credentials, config.TwitchChannel);

        _client.OnLog += OnLog;
        _client.OnJoinedChannel += OnJoinedChannel;
        _client.OnMessageReceived += OnMessageReceived;
        _client.OnConnected += OnConnected;

        Task.Run(() => { _client.Connect(); });
    }

    public event Func<ChatMessage, Task> MessageReceived;

    private void OnLog(object? sender, OnLogArgs e)
    {
        _logger.LogInformation($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
    }

    private void OnConnected(object? sender, OnConnectedArgs e)
    {
        _logger.LogInformation($"Connected to {e.AutoJoinChannel}");
    }

    private void OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
    {
        _logger.LogInformation($"Joined channel {e.Channel}");
    }

    private void OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        _logger.LogInformation($"Received message: {e.ChatMessage.Message}");
        MessageReceived.Invoke(e.ChatMessage);
    }

    private readonly TwitchClient _client;
    private readonly ILogger<TwitchConnector> _logger;
}