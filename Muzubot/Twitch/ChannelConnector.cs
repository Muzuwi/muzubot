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

    /// <summary>
    /// Send a message in the channel of the connector
    /// </summary>
    /// <param name="message">Contents of the message</param>
    public void Send(string message)
    {
        _connector.Client.SendMessage(_channel, message);
    }

    /// <summary>
    /// Reply to a message
    /// </summary>
    /// <param name="message">Contents of the reply</param>
    /// <param name="replyingTo">Message being replied to</param>
    public void Reply(string message, ChatMessage replyingTo)
    {
        _connector.Client.SendReply(_channel, replyingTo.Id, message);
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
        if (e.ChatMessage.Channel != _channel)
        {
            return;
        }

        MessageReceived.Invoke(e.ChatMessage);
    }

    private readonly ILogger<ChannelConnector> _logger;
    private readonly TwitchConnector _connector;
    private readonly string _channel;
}