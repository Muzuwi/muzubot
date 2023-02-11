using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Muzubot.Storage.Models;

[PrimaryKey("UID", "Command")]
public class CommandUsageData
{
    /// <summary>
    /// UID of the user
    /// </summary>
    public string UID { get; set; }

    /// <summary>
    /// Command used
    /// </summary>
    public string Command { get; set; }

    /// <summary>
    /// Last time this command was used
    /// </summary>
    public DateTime? LastUsed { get; set; }

    /// <summary>
    /// Concurrency token
    /// Implementation detail: maps to Postgres' xmin column
    /// </summary>
    [Timestamp]
    public uint Version { get; set; }
}