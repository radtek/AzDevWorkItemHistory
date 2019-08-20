using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            var items = await executor.ExecuteQueryAsync(options.GetQueryId());
            foreach (var workItem in items)
            {
                _stdout.WriteLine(workItem.Id);
            }
            return 0;
        }

        public async Task<int> RunRevisions(RevisionsOptions options)
        {
            var executor = new WorkItemMiner(options.Username, options.PersonalAccessToken, options.GetAzureUri());
            var revisions = executor.GetAllWorkItemRevisionsForProjectAsync(options.Project);

            await foreach (var x in revisions)
            {
                var item = MapWorkItemRevision(x);

                _stdout.WriteLine($"{item.Id},{item.Title},{item.State},{item.ChangeDate},{item.AreaPath},{item.IterationPath},{item.Revision},{item.TeamProject},{item.WorkItemType}");
            }

            return 0;
        }

        public async Task<int> WorkItemDurations(DurationsOptions options)
        {
            var executor = new WorkItemMiner(options.Username, options.PersonalAccessToken, options.GetAzureUri());
            var revisions = executor.GetAllWorkItemRevisionsForProjectAsync(options.Project);
            var workItems = CollectWorkItems(revisions).Result
                .Select(MapWorkItemRevision)
                .GroupBy(x => x.Id)
                .Select(ProcessWorkItem);

            foreach (var item in workItems)
            {
                _stdout.WriteLine($"{item.Id},{item.Title},{item.State},{item.WorkItemType},{item.IterationPath},{item.AreaPath},{item.TeamProject},{DateTimeString(item.Start)},{DateTimeString(item.End)}");
            }

            return 0;

            string DateTimeString(Option<DateTime> item)
            {
                return item.Map(d => d.ToString()).IfNone(string.Empty);
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
            //System.Title
            //System.WorkItemType
            //System.IterationPath
            //System.State
            //System.AreaPath => JEDIv2
            //System.TeamProject => JEDIv2
            //System.ChangedDate

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


        private WorkItemInfo ProcessWorkItem(IEnumerable<WorkItemRevisionInfo> revisions)
        {
            var listed = revisions.ToList();
            var last = listed.Last();
            var (start, end) = GetWorkItemTimeRange(listed);

            return new WorkItemInfo(last.Title, last.WorkItemType, last.IterationPath, last.State, last.AreaPath, last.TeamProject, last.Id, start, end);
        }

        private (Option<DateTime> start, Option<DateTime> end) GetWorkItemTimeRange(IEnumerable<WorkItemRevisionInfo> revisions)
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