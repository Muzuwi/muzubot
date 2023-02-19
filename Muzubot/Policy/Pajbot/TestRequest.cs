using Newtonsoft.Json;

namespace Muzubot.Policy.Pajbot;

public struct TestRequest
{
    public TestRequest(string message)
    {
        Message = message;
    }

    [JsonProperty("message")]
    public string Message { get; set; }
}