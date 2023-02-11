using System.Data;
using Microsoft.Extensions.Logging;
using Muzubot.Commands;
using Muzubot.Storage;
using Muzubot.Storage.Models;
using Npgsql;
using NpgsqlTypes;

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
            .SingleOrDefault(data => data.UID == context.Meta.UserId) ?? new DungeonData
        {
            UID = context.Meta.UserId,
            Experience = 0
        };

        var random = new Random();
        var xp = random.NextInt64(0, 2048);

        dungeonData.Experience += (int)xp;
        _db.Dungeon.Update(dungeonData);
        await _db.SaveChangesAsync();

        context.Reply(
            $"@{context.Meta.Username} | You have gained {xp} experience! You now have {dungeonData.Experience} XP.");
    }

    private readonly ILogger<MyNewCommand> _logger;
    private readonly BotDbContext _db;
}