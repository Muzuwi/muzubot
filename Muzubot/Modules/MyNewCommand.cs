using Microsoft.Extensions.Logging;
using Muzubot.Commands;

namespace Muzubot.Modules;

public class MyNewCommand : BaseCommand
{
    public MyNewCommand(ILogger<MyNewCommand> logger)
    {
        _logger = logger;
    }

    [CommandOpts("ed")]
    public async Task EnterDungeon(CommandContext context)
    {
        _logger.LogInformation("Welcome from command module");
    }

    private readonly ILogger<MyNewCommand> _logger;
}