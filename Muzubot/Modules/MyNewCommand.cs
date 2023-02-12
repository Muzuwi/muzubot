using Microsoft.Extensions.Logging;
using Muzubot.Commands;
using Muzubot.Dungeon;
using Muzubot.Storage;
using Muzubot.Storage.Models;

namespace Muzubot.Modules;

public class MyNewCommand
{
    public MyNewCommand(ILogger<MyNewCommand> logger, BotDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [CommandOpts("ed", 0)]
    public async Task EnterDungeon(CommandContext context)
    {
        var dungeonData = _db.Dungeon
            .SingleOrDefault(data => data.UID == context.Meta.UserId);
        var player = dungeonData is null
            ? new Player(context.Meta.UserId)
            : new Player(dungeonData);

        var random = new Random();
        var xp = random.NextInt64(0, 2048);

        var oldLevel = player.Level;
        player.Experience += (int)xp;
        var newLevel = player.Level;

        _db.Dungeon.AddOrUpdate(player.Model);
        await _db.SaveChangesAsync();

        context.Reply(
            $"@{context.Meta.Username} | You have gained {xp} experience! You now have {player.Experience} XP.");
        if (oldLevel != newLevel)
        {
            context.Reply($"@{context.Meta.Username} leveled up! Level {newLevel}");
        }
    }

    private readonly ILogger<MyNewCommand> _logger;
    private readonly BotDbContext _db;
}