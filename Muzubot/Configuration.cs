using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Muzubot;

public class Configuration
{
    public string TwitchUsername;
    public string TwitchOauth;
    public string TwitchChannel;
    public LogLevel LogLevel;
    public string Prefix;
    public string DbConnectionString;

    private Configuration()
    {
        TwitchUsername = "";
        TwitchOauth = "";
        TwitchChannel = "";
        LogLevel = LogLevel.Information;
        Prefix = "";
        DbConnectionString = "";
    }

    public static Configuration FromEnvironment()
    {
        var username = GetEnvironmentVariableChecked("MUZUBOT_TWITCH_USERNAME");
        var oauth = GetEnvironmentVariableChecked("MUZUBOT_TWITCH_OAUTH");
        var channel = GetEnvironmentVariableChecked("MUZUBOT_TWITCH_CHANNEL");
        var logLevelString = GetEnvironmentVariableOrDefault("MUZUBOT_LOG_LEVEL", "trace");
        var prefix = GetEnvironmentVariableOrDefault("MUZUBOT_PREFIX", "+");
        var dbConnString = GetEnvironmentVariableChecked("MUZUBOT_DB_CONNECTION_STRING");

        if (!Enum.TryParse(logLevelString, true, out LogLevel logLevel))
        {
            throw new InvalidOperationException($"invalid log level {logLevelString} specified");
        }

        var config = new Configuration()
        {
            TwitchUsername = username,
            TwitchOauth = oauth,
            TwitchChannel = channel,
            LogLevel = logLevel,
            Prefix = prefix,
            DbConnectionString = dbConnString
        };
        return config;
    }


    private static string GetEnvironmentVariableChecked(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (value is null)
        {
            throw new InvalidOperationException($"environment variable {key} not present");
        }

        return value;
    }

    private static string GetEnvironmentVariableOrDefault(string key, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (value is null)
        {
            return defaultValue;
        }

        return value;
    }
}