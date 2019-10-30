using System;
using System.Threading.Tasks;
using AzDevWorkItemHistory.Credentials;

namespace WorkItemHistory
{
    public class AuthorizedRunner
    {
        private readonly Runner _runner;

        public AuthorizedRunner(Runner runner)
        {
            _runner = runner;
        }

        public Task<ExitCode> Login(LoginOptions opts) => _runner.Login(opts);
        public Task<ExitCode> Logout(LogoutOptions opts) => _runner.Logout(opts);
        public Task<ExitCode> RunQuery(QueryOptions opts, CredentialV1 cred) => TryIt(opts, () => _runner.RunQuery(opts, cred));
        public Task<ExitCode> RunRevisions(RevisionsOptions opts, CredentialV1 cred) => TryIt(opts, () => _runner.RunRevisions(opts, cred));
        public Task<ExitCode> AllWorkItems(AllWorkItemsOptions opts, CredentialV1 cred) => TryIt(opts, () => _runner.AllWorkItems(opts, cred));
        public Task<ExitCode> WorkItemDurations(DurationsOptions opts, CredentialV1 cred) => TryIt(opts, () => _runner.WorkItemDurations(opts, cred));

        private async Task<ExitCode> TryIt<TOpts>(TOpts opts, Func<Task<ExitCode>> theCall) where TOpts : ProjectOptions
        {
            try
            {
                return await theCall();
            }
            catch (Exception e)
            {
                return e.InnerException is Microsoft.VisualStudio.Services.Common.VssUnauthorizedException ue
                    ? ExitCode.Unauthorized(ue, opts.AzureUri)
                    : ExitCode.GeneralException(e);
            }
        }
    }
}