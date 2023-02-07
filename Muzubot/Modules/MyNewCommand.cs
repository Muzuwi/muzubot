using Microsoft.Extensions.Logging;
using Muzubot.Commands;
using Muzubot.Storage;
using Npgsql;
using NpgsqlTypes;

namespace Muzubot.Modules;

public class MyNewCommand
{
    public MyNewCommand(ILogger<MyNewCommand> logger, IStorageConnector connector, NpgsqlDataSource dbSource)
    {
        _logger = logger;
        _connector = connector;
        _dbSource = dbSource;
    }

    [CommandOpts("ed", 0)]
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

        await using var connection = await _dbSource.OpenConnectionAsync();
        await using var cmd1 = new NpgsqlCommand(
            "INSERT INTO Dungeon (UID, Experience)" +
            "VALUES (($1), ($2))" +
            "ON CONFLICT (UID) DO " +
            "UPDATE SET Experience = Dungeon.Experience + excluded.Experience", connection
        )
        {
            Parameters =
            {
                new() { Value = uid },
                new() { Value = xp }
            }
        };
        var rows = await cmd1.ExecuteNonQueryAsync();
        _logger.LogDebug("{:0} rows affected", rows);

        await using var cmd2 = new NpgsqlCommand("SELECT Experience FROM Dungeon WHERE UID = ($1)", connection)
        {
            Parameters = { new() { Value = uid } }
        };
        await using var reader = await cmd2.ExecuteReaderAsync();
        await reader.ReadAsync();
        var newExp = reader.GetInt32(0);

        context.Reply(
            $"@{context.Meta.Username} | You have gained {xp} experience! You now have {newExp} XP.");
    }

    private readonly ILogger<MyNewCommand> _logger;
    private readonly IStorageConnector _connector;
    private readonly NpgsqlDataSource _dbSource;
}