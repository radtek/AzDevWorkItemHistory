using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using LanguageExt;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using static LanguageExt.Prelude;

namespace WorkItemHistory
{
    public interface ICredentialStore
    {

    }
    public class FileCredentialStore
    {

    }
    public class CredentialManager
    {
        internal bool HasUri(string azureUri)
        {
            throw new NotImplementedException();
        }
    }
    public class Runner
    {
        private readonly TextWriter _stdout;
        private readonly TextWriter _stderr;
        private readonly CredentialManager _credentials;

        public Runner(TextWriter stdout, TextWriter stderr, CredentialManager credentials)
        {
            _stdout = stdout;
            _stderr = stderr;
            _credentials = credentials;
        }

        public async Task<ExitCode> Login(LoginOptions options)
        {
            if (_credentials.HasUri(options.AzureUri))
                return ExitCode.DuplicateUri(options.AzureUri);

            // _credentials.Save(options.AzureUri, options.Username, options.PersonalAccessToken);

            return ExitCode.Success;
        }

        public async Task<ExitCode> RunQuery(QueryOptions options)
        {
            var executor = new WorkItemMiner(options.Username, options.PersonalAccessToken, options.GetAzureUri());
            var items = executor.ExecuteQueryAsync(options.GetQueryId());
            var workItems = CollectWorkItems(items).Result
                .Select(MapWorkItemRevision)
                .ToList();

            WriteCsv(workItems);

            return ExitCode.Success;
        }

        public async Task<ExitCode> AllWorkItems(AllWorkItemsOptions options)
        {
            var executor = new WorkItemMiner(options.Username, options.PersonalAccessToken, options.GetAzureUri());
            var revisions = executor.GetAllWorkItemRevisionsForProjectAsync(options.Project);
            var workItems = CollectWorkItems(revisions).Result
                .Select(MapWorkItemRevision)
                .GroupBy(w => w.Id)
                .Select(g => g.OrderBy(x => x.ChangeDate).Last())
                .Then(items => ApplyFilter(items, options))
                .Select(w => new {w.Id, w.WorkItemType, w.Title})
                .ToList();

            WriteCsv(workItems);

            return ExitCode.Success;
        }

        public async Task<ExitCode> RunRevisions(RevisionsOptions options)
        {
            var executor = new WorkItemMiner(options.Username, options.PersonalAccessToken, options.GetAzureUri());
            var revisions = executor.GetAllWorkItemRevisionsForProjectAsync(options.Project);
            var workItems = CollectWorkItems(revisions).Result
                .Select(MapWorkItemRevision)
                .Then(items => ApplyFilter(items, options))
                .ToList();

            WriteCsv(workItems);

            return ExitCode.Success;
        }

        public async Task<ExitCode> WorkItemDurations(DurationsOptions options)
        {
            var executor = new WorkItemMiner(options.Username, options.PersonalAccessToken, options.GetAzureUri());
            var revisions = executor.GetAllWorkItemRevisionsForProjectAsync(options.Project);
            var workItems = CollectWorkItems(revisions).Result
                .Select(MapWorkItemRevision)
                .GroupBy(x => x.Id)
                .Select(ProcessWorkItem)
                .Then(items => ApplyFilter(items, options))
                .ToList();

            WriteCsv(workItems, cfg => cfg.RegisterClassMap<CsvMaps.WorkItemInfoMap>());

            return ExitCode.Success;
        }

        private IEnumerable<TWorkItemInfo> ApplyFilter<TWorkItemInfo>(IEnumerable<TWorkItemInfo> workItems, ProjectOptions options) where TWorkItemInfo : IWorkItemInfo
        {
            return workItems
                .Where(item => HasFilterOrMatches(item, x => x.IterationPath, x => x.IterationPath))
                .Where(item => HasFilterOrMatches(item, x => x.AreaPath, x => x.AreaPath))
                .Where(item => HasFilterOrMatches(item, x => x.State, x => x.State))
                .Where(item => HasFilterOrMatches(item, x => x.Type, x => x.WorkItemType));

            bool HasFilterOrMatches(TWorkItemInfo item, Func<ProjectOptions, IEnumerable<string>> specifiedFiltersSelector, Func<TWorkItemInfo, string> selector)
            {
                var specifiedFilters = specifiedFiltersSelector(options).ToList();
                var hasFilter = !specifiedFilters.Any();
                var target = selector(item);

                return hasFilter || specifiedFilters.Any(x => x.Equals(target, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        private void WriteCsv<T>(ICollection<T> records, Action<IWriterConfiguration> config = null)
        {
            config ??= _ => {};

            using (var csv = new CsvWriter(_stdout, leaveOpen: true))
            {
                config(csv.Configuration);
                csv.WriteRecords(records);

                _stderr.WriteLine($"Saved {records.Count} items.");
            }
        }

        private static async Task<IEnumerable<WorkItem>> CollectWorkItems(IAsyncEnumerable<WorkItem> items)
        {
            var result = new List<WorkItem>();

            await foreach (var workItem in items)
            {
                result.Add(workItem);
            }

            return result;
        }

        private static WorkItemRevisionInfo MapWorkItemRevision(WorkItem item)
        {
            return new WorkItemRevisionInfo(
                title: item.Fields["System.Title"].ToString(),
                workItemType: item.Fields["System.WorkItemType"].ToString(),
                iterationPath: item.Fields["System.IterationPath"].ToString(),
                state: item.Fields["System.State"].ToString(),
                areaPath: item.Fields["System.AreaPath"].ToString(),
                teamProject: item.Fields["System.TeamProject"].ToString(),
                id: item.Id.Value,
                revision: item.Rev.Value,
                changeDate: DateTime.Parse(item.Fields["System.ChangedDate"].ToString()));
        }

        private static WorkItemInfo ProcessWorkItem(IEnumerable<WorkItemRevisionInfo> revisions)
        {
            var listed = revisions.ToList();
            var last = listed.Last();
            var (start, end) = GetWorkItemTimeRange(listed);

            return new WorkItemInfo(last.Title, last.WorkItemType, last.IterationPath, last.State, last.AreaPath, last.TeamProject, last.Id, start, end);
        }

        private static (Option<DateTime> start, Option<DateTime> end) GetWorkItemTimeRange(IEnumerable<WorkItemRevisionInfo> revisions)
        {
            Option<DateTime> activeDate = None;
            Option<DateTime> endDate = None;

            var lastState = string.Empty;

            foreach (var workItemVersion in revisions)
            {
                if ((lastState.Equals("New", StringComparison.OrdinalIgnoreCase) ||
                    lastState.Equals("Ready", StringComparison.OrdinalIgnoreCase) ||
                    lastState.Equals("UI Design", StringComparison.OrdinalIgnoreCase))
                    &&
                    workItemVersion.State.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    activeDate = Some(workItemVersion.ChangeDate);
                }

                if (!lastState.Equals("Closed", StringComparison.OrdinalIgnoreCase) &&
                    workItemVersion.State.Equals("Closed", StringComparison.OrdinalIgnoreCase))
                {
                    endDate = Some(workItemVersion.ChangeDate);
                }

                lastState = workItemVersion.State;
            }

            return (start: activeDate.IsSome ? activeDate : endDate, end: endDate);
        }
    }
}