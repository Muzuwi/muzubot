using Muzubot.Twitch;
using TwitchLib.Client.Models;

namespace Muzubot.Commands;

public class CommandContext
{
    public CommandContext(ChannelConnector connector, ChatMessage meta)
    {
        _arguments = meta.Message.Split(new[] { " " },
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        _messageMeta = meta;
        _connector = connector;
    }

    public bool ResolveCommand(string prefix, out string command)
    {
        command = "";

        if (_arguments.Length == 0)
        {
            return false;
        }

        //  The command must be in the first argument
        if (!_arguments[0].StartsWith(prefix))
        {
            return false;
        }

        command = _arguments[0][prefix.Length..];
        //  This handles the just-prefix case
        //  There will be no trailing whitespace, as the arguments are cleaned beforehand
        return command.Length != 0;
    }

    /// <summary>
    /// Reply to the message that invoked the command
    /// </summary>
    /// <param name="message">Contents of the reply</param>
    public void Reply(string message) => _connector.Reply(message, _messageMeta);

    public string[] Args => _arguments;
    public ChatMessage Meta => _messageMeta;

    private readonly string[] _arguments;
    private readonly ChatMessage _messageMeta;
    private readonly ChannelConnector _connector;
}