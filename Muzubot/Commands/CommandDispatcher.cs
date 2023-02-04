using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Muzubot.Commands;
using Muzubot.Twitch;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace Muzubot.Commands;

public class CommandDispatcher
{
    public CommandDispatcher(ILogger<CommandDispatcher> logger, Configuration config, TwitchConnector connector)
    {
        _logger = logger;
        _commandPrefix = config.Prefix;
        _modules = new();
        connector.MessageReceived += MessageReceived;
    }

    public void InitializeCommandModules(Assembly commandModuleAssembly, ServiceProvider moduleDepsProvider)
    {
        _moduleDepsProvider = moduleDepsProvider;
        _modules = SearchForCommandModules(commandModuleAssembly);
    }

    private Dictionary<string, Type> SearchForCommandModules(Assembly assembly)
    {
        var types = assembly.ExportedTypes
            .Where(type => type.IsClass && type.IsPublic && type.BaseType == typeof(BaseCommand));

        var modules = new Dictionary<string, Type>();
        foreach (var type in types)
        {
            foreach (var method in type.GetMethods())
            {
                if (!method.HasCommandOptAttribute())
                {
                    continue;
                }

                var attribute = method.GetCommandOptAttribute();
                var command = attribute.Command;
                if (modules.ContainsKey(command))
                {
                    _logger.LogWarning("Duplicate module detected for command {:0}!", command);
                    _logger.LogWarning("First occurence in assembly: {:0}, trying to register in: {:1}",
                        modules[command].AssemblyQualifiedName, type.AssemblyQualifiedName);
                    _logger.LogWarning("The duplicate will be ignored");
                    continue;
                }

                _logger.LogDebug("Registering module for command {:0} in assembly {:1}", command,
                    type.AssemblyQualifiedName);
                modules[command] = type;
            }
        }

        return modules;
    }

    private async Task MessageReceived(ChatMessage args)
    {
        if (!args.Message.StartsWith(_commandPrefix))
        {
            return;
        }

        var context = new CommandContext(args);
        if (!context.ResolveCommand(_commandPrefix, out var command))
        {
            //  Malformed command 
            return;
        }

        if (!_modules.ContainsKey(command))
        {
            //  Command not found
            return;
        }

        var type = _modules[command];
        try
        {
            var obj = ActivatorUtilities.CreateInstance(_moduleDepsProvider!, type);
            var entryMethod = obj
                .GetType()
                .GetMethods()
                .First(m => m.HasCommandOptAttribute());
            await (Task)entryMethod.Invoke(obj, new object[] { context });
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed creating command module: {:0}", ex.ToString());
        }
    }

    private readonly ILogger<CommandDispatcher> _logger;
    private readonly string _commandPrefix;
    private ServiceProvider? _moduleDepsProvider;
    private Dictionary<string, Type> _modules;
}