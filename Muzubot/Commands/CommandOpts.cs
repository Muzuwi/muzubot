namespace Muzubot.Commands;

public class CommandOpts : Attribute
{
    public string Command { get; }

    public CommandOpts(string command)
    {
        Command = command;
    }
}