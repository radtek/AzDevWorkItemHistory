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
    public class Runner
    {
        private readonly TextWriter _stdout;
        private readonly TextWriter _stderr;

        public Runner(TextWriter stdout, TextWriter stderr)
        {
            _stdout = stdout;
            _stderr = stderr;
        }

        public async Task<int> RunQuery(QueryOptions options)
        {
            var executor = new WorkItemMiner(options.Username, options.PersonalAccessToken, options.GetAzureUri());
            var items = executor.ExecuteQueryAsync(options.GetQueryId());
            var workItems = CollectWorkItems(items).Result
                .Select(MapWorkItemRevision)
                .ToList();

            WriteCsv(workItems, cfg => cfg.MemberTypes |= MemberTypes.Fields);

            return 0;
        }

        public async Task<int> RunRevisions(RevisionsOptions options)
        {
            var hasAreaPathFilter = !options.AreaPath.Any();
            var hasIterationPathFilter = !options.IterationPath.Any();

            var executor = new WorkItemMiner(options.Username, options.PersonalAccessToken, options.GetAzureUri());
            var revisions = executor.GetAllWorkItemRevisionsForProjectAsync(options.Project);
            var workItems = CollectWorkItems(revisions).Result
                .Select(MapWorkItemRevision)
                .Where(x => hasAreaPathFilter || options.AreaPath.Any(path => path == x.AreaPath))
                .Where(x => hasIterationPathFilter || options.IterationPath.Any(path => path == x.IterationPath))
                .ToList();


            WriteCsv(workItems, cfg => cfg.MemberTypes |= MemberTypes.Fields);

            return 0;
        }

        public async Task<int> WorkItemDurations(DurationsOptions options)
        {
            var hasAreaPathFilter = !options.AreaPath.Any();
            var hasIterationPathFilter = !options.IterationPath.Any();

            var executor = new WorkItemMiner(options.Username, options.PersonalAccessToken, options.GetAzureUri());
            var revisions = executor.GetAllWorkItemRevisionsForProjectAsync(options.Project);
            var workItems = CollectWorkItems(revisions).Result
                .Select(MapWorkItemRevision)
                .GroupBy(x => x.Id)
                .Select(ProcessWorkItem)
                .Where(x => hasAreaPathFilter || options.AreaPath.Any(path => path == x.AreaPath))
                .Where(x => hasIterationPathFilter || options.IterationPath.Any(path => path == x.IterationPath))
                .ToList();

            WriteCsv(workItems, cfg => cfg.RegisterClassMap<CsvMaps.WorkItemInfoMap>());

            return 0;
        }

        private void WriteCsv<T>(ICollection<T> records, Action<IWriterConfiguration> config)
        {
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

            return (start: activeDate, end: endDate);
        }
    }
}