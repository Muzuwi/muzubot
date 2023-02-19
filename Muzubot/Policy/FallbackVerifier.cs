namespace Muzubot.Policy;

/// <summary>
/// This is a fallback verifier to use when pajbot API for the channel was not provided.
/// This always returns a positive verification status.
/// </summary>
public class FallbackVerifier : IMessageVerifier
{
    public FallbackVerifier()
    {
    }

    public async Task<bool> Verify(string message)
    {
        return true;
    }
}