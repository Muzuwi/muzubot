using System.Reflection;

namespace Muzubot.Commands;

public static class MethodInfoExtensions
{
    public static CommandOpts GetCommandOptAttribute(this MethodInfo method)
    {
        return method.GetCustomAttributes().OfType<CommandOpts>().First();
    }

    public static bool HasCommandOptAttribute(this MethodInfo method)
    {
        return method.GetCustomAttributes().OfType<CommandOpts>().Any();
    }
}