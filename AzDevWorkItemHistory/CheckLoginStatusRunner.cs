using System;
using System.Threading.Tasks;
using AzDevWorkItemHistory.Credentials;

namespace WorkItemHistory
{
    public class CheckLoginStatusRunner
    {
        private readonly CredentialManager _credentialManager;
        private readonly AuthorizedRunner _runner;

        public CheckLoginStatusRunner(CredentialManager credentialManager, AuthorizedRunner runner)
        {
            _credentialManager = credentialManager;
            _runner = runner;
        }

        public Task<ExitCode> Login(LoginOptions opts) => _runner.Login(opts);
        public Task<ExitCode> RunQuery(QueryOptions opts) => CheckLoginAndRun(opts, c => _runner.RunQuery(opts, c));
        public Task<ExitCode> RunRevisions(RevisionsOptions opts) => CheckLoginAndRun(opts, c => _runner.RunRevisions(opts, c));
        public Task<ExitCode> AllWorkItems(AllWorkItemsOptions opts) => CheckLoginAndRun(opts, c => _runner.AllWorkItems(opts, c));
        public Task<ExitCode> WorkItemDurations(DurationsOptions opts) => CheckLoginAndRun(opts, c => _runner.WorkItemDurations(opts, c));

        private Task<ExitCode> CheckLoginAndRun<TOpts>(TOpts opts, Func<CredentialV1, Task<ExitCode>> theCall) where TOpts : ProjectOptions
        {
            return _credentialManager.Get(opts.AzureUri).Match(
                Some: theCall,
                None: Task.FromResult(ExitCode.NeedToLogin(opts.AzureUri)));
        }
    }
}