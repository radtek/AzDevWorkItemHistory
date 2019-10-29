using System;
using System.Linq;
using LanguageExt;

namespace AzDevWorkItemHistory.Credentials
{
    public class CredentialManager
    {
        private readonly ICredentialStore _store;
        private readonly CredentialsV1 _credentials;

        public CredentialManager(ICredentialStore store)
        {
            _store = store;
            _credentials = _store.Load();
        }

        public bool HasUri(Uri azureUri)
        {
            return _credentials.Credentials.Any(c => c.AzureUri == azureUri.ToString());
        }

        public void Add(CredentialV1 credential)
        {
            if (HasUri(credential.GetUri()))
                throw new InvalidOperationException($"Cannot have duplicate URIs ({credential.AzureUri})");

            _credentials.Credentials.Add(credential);
        }

        public void Remove(Uri uri)
        {
            _credentials.Credentials.RemoveAll(c => c.AzureUri == uri.ToString());
        }

        public Option<CredentialV1> Get(Uri uri)
        {
            return _credentials.Credentials.SingleOrDefault(c => c.AzureUri == uri.ToString())
             ?? Option<CredentialV1>.None;
        }

        public void Save()
        {
            _store.Store(_credentials);
        }
    }
}