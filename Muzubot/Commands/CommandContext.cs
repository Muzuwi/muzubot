using Muzubot.Storage;
using Muzubot.Twitch;
using TwitchLib.Client.Models;

namespace Muzubot.Commands;

public class CommandContext
{
    public CommandContext(ChannelConnector connector, ChatMessage meta, UserData data)
    {
        _arguments = meta.Message.Split(new[] { " " },
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        _messageMeta = meta;
        _connector = connector;
        _data = data;
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

    /// <summary>
    /// Try using the specified command. This checks whether the user is allowed
    /// to use the command, either by privileges or cooldown.
    /// </summary>
    /// <param name="commandInfo">Command being used</param>
    /// <returns>Whether the command was used successfully</returns>
    public bool UseCommand(CommandOpts commandInfo)
    {
        var command = commandInfo.Command;

        //  User has not used this command yet
        if (!_data.LastCommandUse.ContainsKey(command))
        {
            _data.LastCommandUse[command] = DateTime.UtcNow;
            return true;
        }

        var now = DateTime.UtcNow;
        var lastUsage = _data.LastCommandUse[command];
        var difference = now - lastUsage;

        //  Cooldown has not yet expired
        var cooldown = new TimeSpan(0, 0, 0, commandInfo.Cooldown);
        if (difference < cooldown)
        {
            return false;
        }

        //  Successful command use - store the time of use
        _data.LastCommandUse[command] = DateTime.UtcNow;
        return true;
    }

    public string[] Args => _arguments;
    public ChatMessage Meta => _messageMeta;
    public UserData Data => _data;

    private readonly string[] _arguments;
    private readonly ChatMessage _messageMeta;
    private readonly ChannelConnector _connector;
    private readonly UserData _data;
}