using Muzubot.Storage.Models;

namespace Muzubot.Commands;

public class CommandUsage
{
    public CommandUsage(CommandUsageData model)
    {
        _model = model;
    }

    public CommandUsage(string uid, string commandName)
    {
        _model = new CommandUsageData
        {
            UID = uid,
            Command = commandName,
            LastUsed = DateTime.UtcNow
        };
    }

    public CommandUsageData Model => _model;

    /// <summary>
    /// Try using the specified command. This checks whether the user is allowed
    /// to use the command, either by privileges or cooldown.
    /// </summary>
    /// <param name="commandInfo">Command being used</param>
    /// <returns>Whether the command was used successfully</returns>
    public bool UseCommand(CommandOpts commandInfo)
    {
        var command = commandInfo.Command;
        var now = DateTime.UtcNow;

        //  User has not used this command yet
        if (_model.LastUsed is null)
        {
            _model.LastUsed = now;
            return true;
        }

        var difference = now - _model.LastUsed;

        //  Cooldown has not yet expired
        var cooldown = new TimeSpan(0, 0, 0, commandInfo.Cooldown);
        if (difference < cooldown)
        {
            return false;
        }

        //  Successful command use - store the time of use
        _model.LastUsed = now;
        return true;
    }

    private readonly CommandUsageData _model;
}