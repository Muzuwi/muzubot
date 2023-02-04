using Microsoft.Extensions.Logging;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace Muzubot.Twitch;

public class ChannelConnector
{
    public ChannelConnector(ILogger<ChannelConnector> logger, TwitchConnector connector, Configuration config)
    {
        _logger = logger;
        _connector = connector;
        _channel = config.TwitchChannel;

        _connector.Client.OnJoinedChannel += OnJoinedChannel;
        _connector.Client.OnMessageReceived += OnMessageReceived;
        _connector.Client.JoinChannel(_channel);
    }

    public event Func<ChatMessage, Task> MessageReceived;
    public string Channel => _channel;

    private void OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
    {
        if (e.Channel != _channel)
        {
            return;
        }

        _logger.LogInformation($"Joined channel {e.Channel}");
    }

    private void OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        _logger.LogDebug("message received: {:0}", e.ChatMessage);
        if (e.ChatMessage.Channel != _channel)
        {
            return;
        }

        _logger.LogInformation($"#{_channel} => Received message: {e.ChatMessage.Message}");
        MessageReceived.Invoke(e.ChatMessage);
    }

    private readonly ILogger<ChannelConnector> _logger;
    private readonly TwitchConnector _connector;
    private readonly string _channel;
}