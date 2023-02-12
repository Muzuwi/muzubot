using Muzubot.Storage.Models;

namespace Muzubot.Dungeon;

public class Player
{
    public Player(string uid)
    {
        _model = new DungeonData
        {
            UID = uid,
            Experience = 0,
            Gold = Constants.StartingGold,
            UnspentPoints = Constants.StartingSkillPoints
        };
    }

    public Player(DungeonData model)
    {
        _model = model;
    }

    public int Experience
    {
        get => _model.Experience;
        set => _model.Experience = value;
    }

    public int Level => (int)Constants.ExperienceToLevel(Experience);

    public DungeonData Model => _model;
    private readonly DungeonData _model;
}