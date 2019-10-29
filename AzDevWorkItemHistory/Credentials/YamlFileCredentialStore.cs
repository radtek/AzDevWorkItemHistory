using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AzDevWorkItemHistory.Credentials
{
    public class YamlFileCredentialStore : ICredentialStore
    {
        private readonly string _location;

        public YamlFileCredentialStore(string location)
        {
            _location = location;
        }

        public void Store(CredentialsV1 credentialsV1)
        {
            var serializer = new SerializerBuilder()
               .WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();

            File.WriteAllText(_location, serializer.Serialize(credentialsV1), Encoding.UTF8);
        }

        public CredentialsV1 Load()
        {
            if (!File.Exists(_location))
                return new CredentialsV1();

            var deserializer = new DeserializerBuilder()
               .WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();

            return deserializer.Deserialize<CredentialsV1>(File.ReadAllText(_location, Encoding.UTF8));
        }
    }
}