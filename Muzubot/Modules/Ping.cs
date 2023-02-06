using System.Diagnostics;
using Muzubot.Commands;

namespace Muzubot.Modules;

public class Ping
{
    [CommandOpts("ping", 60)]
    public async Task EnterDungeon(CommandContext context)
    {
        var tmiTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(context.Meta.TmiSentTs)).LocalDateTime;
        var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
        var latency = DateTime.Now - tmiTime;
        context.Reply(
            $"Pong! Uptime: {uptime:dd}d{uptime:hh}h{uptime:mm}m, latency to TMI: {latency.TotalMilliseconds}ms");
    }
}