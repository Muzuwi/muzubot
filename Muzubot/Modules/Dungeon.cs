using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Muzubot.Commands;
using Muzubot.Dungeon;
using Muzubot.Storage;
using Muzubot.Storage.Models;

namespace Muzubot.Modules;

public class DungeonModule
{
    public DungeonModule(ILogger<DungeonModule> logger, BotDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [CommandOpts("ed", 0)]
    public async Task EnterDungeon(CommandContext context)
    {
        var player = FetchOrCreatePlayerData(context.Meta.UserId);
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

    [CommandOpts("stats", 60)]
    public async Task Stats(CommandContext context)
    {
        var player = FetchOrCreatePlayerData(context.Meta.UserId);

        var levelText = $"Level {player.Level}";
        var experienceText = $"{player.Experience} / {Constants.LevelToExperience(player.Level + 1)} XP 💠";
        var goldText = $"🪙 {player.Model.Gold}";
        var skillsText =
            $"⚔ {player.Model.AttackPoints} 🛡 {player.Model.DefensePoints} 💨 {player.Model.AgilityPoints} 🎲 {player.Model.LuckPoints}";
        var availablePointsText = $"🎓 {player.Model.UnspentPoints}";

        context.Reply(
            $"@{context.Meta.Username} | {levelText} | {experienceText} | {goldText} | {skillsText} | {availablePointsText}");
    }

    private Player FetchOrCreatePlayerData(string uid)
    {
        var dungeonData = _db.Dungeon
            .SingleOrDefault(data => data.UID == uid);

        var player = dungeonData is null
            ? new Player(uid)
            : new Player(dungeonData);

        return player;
    }

    private readonly ILogger<DungeonModule> _logger;
    private readonly BotDbContext _db;
}