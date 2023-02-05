using Microsoft.Extensions.Logging;
using Muzubot.Commands;
using Muzubot.Storage;

namespace Muzubot.Modules;

public class MyNewCommand
{
    public MyNewCommand(ILogger<MyNewCommand> logger, IStorageConnector connector)
    {
        _logger = logger;
        _connector = connector;
    }

    [CommandOpts("ed")]
    public async Task EnterDungeon(CommandContext context)
    {
        var uid = context.Meta.UserId;
        var userData = await _connector.FetchOrCreateUserData(uid);
        if (userData is null)
        {
            _logger.LogError($"Failed fetching user data for user {uid}");
            return;
        }

        var random = new Random();
        var xp = random.NextInt64(0, 2048);
        userData.Experience += xp;
        await _connector.CommitUserData(userData);

        context.Reply(
            $"@{context.Meta.Username} | You have gained {xp} experience! You now have {userData.Experience} XP.");
    }

    private readonly ILogger<MyNewCommand> _logger;
    private readonly IStorageConnector _connector;
}