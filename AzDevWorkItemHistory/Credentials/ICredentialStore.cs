namespace AzDevWorkItemHistory.Credentials
{
    public interface ICredentialStore
    {
        void Store(CredentialsV1 credentialsV1);
        CredentialsV1 Load();
    }
}