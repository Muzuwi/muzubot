using System.Net.Http.Headers;
using System.Text;
using Muzubot.Utilities;
using Newtonsoft.Json;

namespace Muzubot.Policy.Pajbot;

public class PajbotVerifier : IMessageVerifier
{
    public PajbotVerifier(string pajbotRootUrl)
    {
        _pajbotRoot = pajbotRootUrl;
    }

    public async Task<bool> Verify(string message)
    {
        var response = await VerifyMessage(message);
        return !response.Banned;
    }

    public async Task<TestResponse> VerifyMessage(string message)
    {
        var test = new TestRequest(message);

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent",
            $"muzubot/0.0.0 ({GitInfo.GetBuildBranch()}, {GitInfo.GetBuildCommitHash()})");

        var content = new StringContent(JsonConvert.SerializeObject(test), Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json"));
        var response = await client.PostAsync($"{_pajbotRoot}{VerificationEndpoint}", content);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TestResponse>(resultJson);
    }

    private const string VerificationEndpoint = "/api/v1/banphrases/test";

    private readonly string _pajbotRoot;
}