using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Muzubot.Twitch;

public class TwitchConnector
{
    public TwitchConnector(ILogger<TwitchConnector> logger, Configuration config)
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
        _client.Initialize(credentials);
        _client.OnLog += OnLog;
    }

    public async Task Connect()
    {
        SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
        EventHandler<OnConnectedArgs> handler = (_, _) => { semaphore.Release(1); };

        _client.OnConnected += handler;
        _ = Task.Run(() => _client.Connect());
        await semaphore.WaitAsync();
        _client.OnConnected -= handler;
    }

    public TwitchClient Client => _client;

    private void OnLog(object? sender, OnLogArgs e)
    {
        _logger.LogInformation($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
    }

    private readonly TwitchClient _client;
    private readonly ILogger<TwitchConnector> _logger;
}