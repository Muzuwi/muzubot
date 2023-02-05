namespace Muzubot.Storage;

public interface IStorageConnector
{
    public Task<UserData?> FetchUserData(string uid);
    public Task<UserData?> FetchOrCreateUserData(string uid);
    public Task<bool> CommitUserData(UserData data);
}