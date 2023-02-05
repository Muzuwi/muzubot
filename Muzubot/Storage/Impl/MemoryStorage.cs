namespace Muzubot.Storage.Impl;

public class MemoryStorage : IStorageConnector
{
    public MemoryStorage()
    {
        _lock = new();
        _userData = new();
    }

    public async Task<UserData?> FetchUserData(string uid)
    {
        lock (_lock)
        {
            if (!_userData.ContainsKey(uid))
            {
                return null;
            }

            return _userData[uid];
        }
    }

    public async Task<UserData?> FetchOrCreateUserData(string uid)
    {
        lock (_lock)
        {
            if (_userData.ContainsKey(uid))
            {
                return _userData[uid];
            }

            var data = UserData.CreateDefaultForUser(uid);
            _userData[uid] = data;
            return data;
        }
    }

    public async Task<bool> CommitUserData(UserData data)
    {
        lock (_lock)
        {
            _userData[data.UID] = data;
            return true;
        }
    }

    private object _lock;
    private Dictionary<string, UserData> _userData;
}