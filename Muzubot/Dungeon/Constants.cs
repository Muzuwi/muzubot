namespace Muzubot.Dungeon;

public static class Constants
{
    public const long LevelFormulaBase = 2000;
    public const double LevelFormulaExponent = 1.09;
    public const int StartingLevel = 1;
    public const int StartingGold = 0;
    public const int StartingSkillPoints = 1;

    public static long ExperienceToLevel(int experience)
    {
        //  exp = base * z^level
        //  exp/base = z^level
        //  log<z>(exp/base) = level
        return (long)Math.Log((double)experience / LevelFormulaBase, LevelFormulaExponent);
    }

    public static long LevelToExperience(long level)
    {
        return (long)Math.Floor(LevelFormulaBase * Math.Pow(LevelFormulaExponent, level));
    }
}