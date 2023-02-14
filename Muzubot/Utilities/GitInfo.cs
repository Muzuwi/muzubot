using System.Reflection;

namespace Muzubot.Utilities;

public static class GitInfo
{
    public static string? GetBuildCommitHash()
    {
        return Assembly
            .GetEntryAssembly()?
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attr => attr.Key == "GitHash")?.Value;
    }

    public static string? GetBuildBranch()
    {
        return Assembly
            .GetEntryAssembly()?
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attr => attr.Key == "GitBranch")?.Value;
    }
}