namespace Muzubot.Policy;

public interface IMessageVerifier
{
    /// <summary>
    /// Verify whether the given message is considered safe to post
    /// </summary>
    /// <param name="message">Message to check</param>
    /// <returns>Is the message safe?</returns>
    public Task<bool> Verify(string message);
}