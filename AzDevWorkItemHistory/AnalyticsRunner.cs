using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using static WorkItemHistory.AnalyticsHelpers;

namespace WorkItemHistory
{
    public class AnalyticsRunner
    {
        private readonly TelemetryClient _telemetry;
        private readonly CheckLoginStatusRunner _runner;

        public AnalyticsRunner(TelemetryClient telemetry, CheckLoginStatusRunner runner)
        {
            _telemetry = telemetry;
            _runner = runner;
        }

        public Task<ExitCode> Login(LoginOptions opts) => TrackEvent(opts, () => _runner.Login(opts));
        public Task<ExitCode> Logout(LogoutOptions opts) => TrackEvent(opts, () => _runner.Logout(opts));
        public Task<ExitCode> RunQuery(QueryOptions opts) => TrackEvent(opts, () => _runner.RunQuery(opts));
        public Task<ExitCode> RunRevisions(RevisionsOptions opts) => TrackEvent(opts, () => _runner.RunRevisions(opts));
        public Task<ExitCode> AllWorkItems(AllWorkItemsOptions opts) => TrackEvent(opts, () => _runner.AllWorkItems(opts));
        public Task<ExitCode> WorkItemDurations(DurationsOptions opts) => TrackEvent(opts, () => _runner.WorkItemDurations(opts));

        private Task<ExitCode> TrackEvent<TOpts>(TOpts opts, Func<Task<ExitCode>> theCall) where TOpts : AzureOptions
        {
            using (_telemetry.StartOperation<RequestTelemetry>(GetVerb<TOpts>()))
            {
                return theCall().Then(exit => {
                    _telemetry.TrackEvent(GetVerb<TOpts>(), GetEventData(opts));
                    return exit;
                });
            }
        }
    }
}