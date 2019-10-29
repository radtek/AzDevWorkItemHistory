using System;
using System.IO;
using AzDevWorkItemHistory.Credentials;
using NUnit.Framework;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tests
{
    public class YamlFileStoreTests
    {
        [Test]
        public void ReturnsEmptyWhenNoFile()
        {
            using var tmpFile = TempFile.Init("credentials.yml");

            var credentials = new YamlFileCredentialStore(tmpFile.Path).Load();

            Assert.That(credentials.Credentials, Is.Empty);
        }
        [Test]
        public void RoundTripsCredentials()
        {
            using var tmpFile = TempFile.Init("credentials.yml");

            new YamlFileCredentialStore(tmpFile.Path).Store(new CredentialsV1
            {
                Credentials =
                {
                    CredentialV1.CreateFromPlainText(new Uri("https://example.org"), "the_user", "s3cr3+")
                }
            });

            var credentials = new YamlFileCredentialStore(tmpFile.Path).Load();

            Assert.That(credentials.Credentials[0].GetPlainTextPersonalAccessToken(), Is.EqualTo("s3cr3+"));
            Assert.That(credentials.Credentials[0].Username, Is.EqualTo("the_user"));
        }

        [Test]
        public void DeserializesV1Configuration()
        {
            var input = new StringReader(Document);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var credentials = deserializer.Deserialize<CredentialsV1>(input);

            Assert.AreEqual(1, credentials.Version);
            Assert.AreEqual(1, credentials.Credentials.Count);
            Assert.AreEqual("https://example.org", credentials.Credentials[0].AzureUri);
            Assert.AreEqual("abc123", credentials.Credentials[0].PersonalAccessToken);
        }

        private const string Document = @"---
version: 1
credentials:
    - azureUri: 'https://example.org'
      username: the_user
      personalAccessToken: 'abc123'
";
    }
}