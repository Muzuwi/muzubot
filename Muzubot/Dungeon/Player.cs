using Muzubot.Storage.Models;

namespace Muzubot.Dungeon;

public class Player
{
    public Player(string uid)
    {
        _model = new DungeonData
        {
            UID = uid,
            Level = 1,
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
        set
        {
            _model.Experience = value;

            var toNextLevel = Constants.LevelToExperience(Level);
            while (_model.Experience >= toNextLevel)
            {
                _model.Experience -= (int)toNextLevel;
                LevelUp();
                toNextLevel = Constants.LevelToExperience(Level);
            }
        }
    }

    public int Level
    {
        get => _model.Level;
        private set => _model.Level = value;
    }

    public DungeonData Model => _model;

    private void LevelUp()
    {
        Level += 1;
    }

    private readonly DungeonData _model;
}