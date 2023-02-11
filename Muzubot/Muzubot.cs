using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Muzubot.Commands;
using Muzubot.Storage;
using Muzubot.Twitch;

namespace Muzubot;

public class Muzubot
{
    public Muzubot(Configuration config)
    {
        _config = config;
    }

    public async Task Run()
    {
        _serviceProvider = ConfigureServices();
        var twitchConnector = _serviceProvider.GetRequiredService<TwitchConnector>();
        await twitchConnector.Connect();

        var dispatcher = _serviceProvider.GetRequiredService<CommandDispatcher>();
        dispatcher.InitializeCommandModules(Assembly.GetEntryAssembly()!, _serviceProvider);

        await Task.Delay(-1);
    }

    private ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton(_config)
            .AddSingleton<TwitchConnector>()
            .AddSingleton<ChannelConnector>()
            .AddSingleton<CommandDispatcher>()
            .AddTransient<BotDbContext>()
            .AddLogging(config => config
                .AddConsole()
                .SetMinimumLevel(_config.LogLevel))
            .BuildServiceProvider();
    }

    private Configuration _config;
    private ServiceProvider? _serviceProvider;
}