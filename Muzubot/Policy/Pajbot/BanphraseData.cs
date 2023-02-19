using Newtonsoft.Json;

namespace Muzubot.Policy.Pajbot;

public struct BanphraseData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("phrase")]
    public string Phrase { get; set; }

    [JsonProperty("length")]
    public int Length { get; set; }

    [JsonProperty("permanent")]
    public bool Permanent { get; set; }

    [JsonProperty("operator")]
    public string Operator { get; set; }

    [JsonProperty("case_sensitive")]
    public bool CaseSensitive { get; set; }
}