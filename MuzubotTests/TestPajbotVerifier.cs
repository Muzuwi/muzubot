using Muzubot.Policy.Pajbot;
using NUnit.Framework;

namespace MuzubotTests
{
    [TestFixture]
    public class TestPajbotVerifier
    {
        [SetUp]
        public void SetUp()
        {
            _verifier = new PajbotVerifier("https://forsen.tv");
        }

        [Test]
        public async Task TestMessageVerification()
        {
            var result = await _verifier.VerifyMessage("KEKW");
            Assert.True(result.Banned);
            Assert.NotNull(result.BanphraseData);
        }

        private PajbotVerifier _verifier;
    }
}