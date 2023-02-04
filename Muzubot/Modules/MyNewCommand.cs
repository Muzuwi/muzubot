using Microsoft.Extensions.Logging;
using Muzubot.Commands;

namespace Muzubot.Modules;

public class MyNewCommand
{
    public MyNewCommand(ILogger<MyNewCommand> logger)
    {
        _logger = logger;
    }

    [CommandOpts("ed")]
    public async Task EnterDungeon(CommandContext context)
    {
        _logger.LogInformation("Welcome from command module");
        context.Reply($"It is now {DateTime.Now}");
    }

    private readonly ILogger<MyNewCommand> _logger;
}