namespace Muzubot.Commands;

public class CommandOpts : Attribute
{
    public string Command { get; }

    /// <summary>
    /// Cooldown of the command (in seconds)
    /// </summary>
    public int Cooldown { get; }

    public CommandOpts(string command)
    {
        Command = command;
        Cooldown = 30;
    }

    public CommandOpts(string command, int cooldown)
    {
        Command = command;
        Cooldown = cooldown;
    }
}