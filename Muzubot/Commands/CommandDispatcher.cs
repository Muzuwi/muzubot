using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Muzubot.Storage;
using Muzubot.Twitch;
using TwitchLib.Client.Models;

namespace Muzubot.Commands;

public class CommandDispatcher
{
    public CommandDispatcher(ILogger<CommandDispatcher> logger, Configuration config, ChannelConnector connector)
    {
        _logger = logger;
        _commandPrefix = config.Prefix;
        _modules = new();
        _connector = connector;
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
            .Where(type => type.IsClass && type.IsPublic);

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
        var context = CreateCommandContext(args, out var commandName);
        if (context is null)
        {
            return;
        }

        var moduleObject = CreateModuleInstanceForCommand(commandName);
        if (moduleObject is null)
        {
            return;
        }

        //  Get the command handler method from the module object
        var entryMethod = GetModuleMethodForCommand(moduleObject, commandName);
        if (entryMethod is null)
        {
            return;
        }

        var commandInfo = entryMethod.GetCommandOptAttribute();
        if (!await UpdateCommandCooldown(args, commandInfo))
        {
            return;
        }

        await InvokeModuleCommandHandler(moduleObject, entryMethod, context);
    }

    private CommandContext? CreateCommandContext(ChatMessage message, out string command)
    {
        if (!message.Message.StartsWith(_commandPrefix))
        {
            command = "";
            return null;
        }

        var context = new CommandContext(_connector, message);
        if (!context.ResolveCommand(_commandPrefix, out command))
        {
            //  Malformed command 
            return null;
        }

        return context;
    }

    private object? CreateModuleInstanceForCommand(string command)
    {
        if (!_modules.ContainsKey(command))
        {
            //  Command not found
            return null;
        }

        try
        {
            var type = _modules[command];
            var obj = ActivatorUtilities.CreateInstance(_moduleDepsProvider!, type);
            return obj;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Instantiation of module for command {:0} failed: {:1}", command, ex.ToString());
            return null;
        }
    }

    private MethodInfo? GetModuleMethodForCommand(object module, string command)
    {
        return module
            .GetType()
            .GetMethods()
            .FirstOrDefault(m => m.HasCommandOptAttribute());
    }

    private async Task InvokeModuleCommandHandler(object module, MethodBase method, CommandContext commandContext)
    {
        try
        {
            //  Execute the command handler
            await (Task)method.Invoke(module, new object[] { commandContext });
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Command module execution threw an exception: {:0}", ex.ToString());
        }
    }

    private BotDbContext CreateDbContext()
    {
        var obj = ActivatorUtilities.CreateInstance(_moduleDepsProvider!, typeof(BotDbContext));
        return obj as BotDbContext ?? throw new InvalidOperationException("Could not instantiate a DB context");
    }

    private async Task<bool> UpdateCommandCooldown(ChatMessage message, CommandOpts commandInfo)
    {
        try
        {
            await using var context = CreateDbContext();

            var cooldownData = context.CommandUsage
                .SingleOrDefault(usage => usage.Command == commandInfo.Command && usage.UID == message.UserId);

            var usage = cooldownData is null
                ? new CommandUsage(message.UserId, commandInfo.Command)
                : new CommandUsage(cooldownData);

            var canUseCommand = usage.UseCommand(commandInfo);
            //  If the user can't use the command, we can skip the rest
            if (!canUseCommand)
            {
                return false;
            }

            //  Update cooldown data in the db 
            context.CommandUsage.AddOrUpdate(usage.Model);
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Updating cooldown failed: {:0}", ex.ToString());
            return false;
        }
    }

    private readonly ILogger<CommandDispatcher> _logger;
    private readonly string _commandPrefix;
    private ServiceProvider? _moduleDepsProvider;
    private Dictionary<string, Type> _modules;
    private ChannelConnector _connector;
}