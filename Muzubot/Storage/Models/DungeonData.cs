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
    /// User level
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Experience points
    /// </summary>
    public int Experience { get; set; }

    /// <summary>
    /// Gold (currently unused)
    /// </summary>
    public int Gold { get; set; }

    /// <summary>
    /// Attack
    /// </summary>
    public int AttackPoints { get; set; }

    /// <summary>
    /// Defense
    /// </summary>
    public int DefensePoints { get; set; }

    /// <summary>
    /// Agility
    /// </summary>
    public int AgilityPoints { get; set; }

    /// <summary>
    /// Luck
    /// </summary>
    public int LuckPoints { get; set; }

    /// <summary>
    /// Available points to distribute
    /// </summary>
    public int UnspentPoints { get; set; }

    /// <summary>
    /// Concurrency token
    /// Implementation detail: maps to Postgres' xmin column
    /// </summary>
    [Timestamp]
    public uint Version { get; set; }
}