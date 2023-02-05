namespace Muzubot.Storage;

public class UserData
{
    public string UID { get; private set; }

    public long Experience { get; set; }

    public static UserData CreateDefaultForUser(string uid)
    {
        return new UserData()
        {
            UID = uid,
            Experience = 0
        };
    }
}