using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Muzubot.Commands;
using Muzubot.Storage;
using Muzubot.Storage.Impl;
using Muzubot.Twitch;
using Npgsql;

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
            .AddScoped<IStorageConnector, MemoryStorage>()
            .AddSingleton(_ => NpgsqlDataSource.Create(_config.DbConnectionString))
            .AddLogging(config => config
                .AddConsole()
                .SetMinimumLevel(_config.LogLevel))
            .BuildServiceProvider();
    }

    private Configuration _config;
    private ServiceProvider? _serviceProvider;
}