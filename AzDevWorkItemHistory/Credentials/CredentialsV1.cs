using System;
using System.Collections.Generic;

namespace AzDevWorkItemHistory.Credentials
{
    public class CredentialsV1
    {
        public int Version { get; set; } = 1;
        public List<CredentialV1> Credentials { get; set; } = new List<CredentialV1>();
    }

    public class CredentialV1
    {
        public string AzureUri { get; set; }
        public string Username { get; set; }
        public string PersonalAccessToken { get; set; }

        public static CredentialV1 CreateFromPlainText(Uri azureUri, string username, string plainTextPersonalAccessToken)
        {
            return new CredentialV1 {
                AzureUri = azureUri.ToString(),
                Username = username,
                PersonalAccessToken = EncryptionUtility.EncryptString(plainTextPersonalAccessToken)
            };
        }

        public string GetPlainTextPersonalAccessToken()
        {
            return EncryptionUtility.DecryptString(PersonalAccessToken);
        }

        public Uri GetUri()
        {
            return new Uri(AzureUri);
        }
    }
}