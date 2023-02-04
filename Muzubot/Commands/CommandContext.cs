using TwitchLib.Client.Models;

namespace Muzubot.Commands;

public class CommandContext
{
    public CommandContext(ChatMessage meta)
    {
        _arguments = meta.Message.Split(new[] { " " },
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        _messageMeta = meta;
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


    public string[] Args => _arguments;
    public ChatMessage Meta => _messageMeta;

    private readonly string[] _arguments;
    private readonly ChatMessage _messageMeta;
}