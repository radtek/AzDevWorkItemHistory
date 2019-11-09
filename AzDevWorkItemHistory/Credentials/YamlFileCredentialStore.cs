using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AzDevWorkItemHistory.Credentials
{
    public class YamlFileCredentialStore : ICredentialStore
    {
        private readonly string _fullPath;

        public YamlFileCredentialStore(string location)
        {
            _fullPath = location;
            if (!Path.IsPathFullyQualified(location))
                _fullPath = Path.GetFullPath(_fullPath);
        }

        public void Store(CredentialsV1 credentialsV1)
        {
            var directory = Path.GetDirectoryName(_fullPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var serializer = new SerializerBuilder()
               .WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();

            File.WriteAllText(_fullPath, serializer.Serialize(credentialsV1), Encoding.UTF8);
        }

        public CredentialsV1 Load()
        {
            if (!Directory.Exists(Path.GetDirectoryName(_fullPath)))
                return new CredentialsV1();

            if (!File.Exists(_fullPath))
                return new CredentialsV1();

            var deserializer = new DeserializerBuilder()
               .WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();

            return deserializer.Deserialize<CredentialsV1>(File.ReadAllText(_fullPath, Encoding.UTF8));
        }
    }
}