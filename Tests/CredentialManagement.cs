using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using WorkItemHistory;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tests
{
    public class CredentialsV1
    {
        public int Version { get; set; }
        public List<CredentialV1> Credentials { get; set; }
    }

    public class CredentialV1
    {
        public string AzureUri { get; set; }
        public string PersonalAccessToken { get; set; }
    }
    public class CredentialManagement
    {
        [Test]
        public void HasUriIsFalseWhenDoesntHaveUri()
        {
            var input = new StringReader(Document);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var creds = deserializer.Deserialize<CredentialsV1>(input);

            Assert.AreEqual(1, creds.Version);
            Assert.AreEqual(1, creds.Credentials.Count);
            Assert.AreEqual("https://example.org", creds.Credentials[0].AzureUri);
            Assert.AreEqual("abc123", creds.Credentials[0].PersonalAccessToken);
        }

        private const string Document = @"---
version: 1
credentials:
    - azureUri: 'https://example.org'
      personalAccessToken: 'abc123'
";
    }
}