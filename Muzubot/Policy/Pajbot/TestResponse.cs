using Newtonsoft.Json;

namespace Muzubot.Policy.Pajbot;

public struct TestResponse
{
    [JsonProperty("banned")]
    public bool Banned { get; set; }

    [JsonProperty("input_message")]
    public string InputMessage { get; set; }

    [JsonProperty("banphrase_data")]
    public BanphraseData? BanphraseData { get; set; }
}