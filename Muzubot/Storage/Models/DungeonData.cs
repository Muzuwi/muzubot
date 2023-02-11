using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Muzubot.Storage.Models;

[PrimaryKey("UID")]
public class DungeonData
{
    /// <summary>
    /// UID of the user
    /// </summary>
    public string UID { get; set; }

    /// <summary>
    /// Experience points
    /// </summary>
    public int Experience { get; set; }

    /// <summary>
    /// Concurrency token
    /// Implementation detail: maps to Postgres' xmin column
    /// </summary>
    [Timestamp]
    public uint Version { get; set; }
}