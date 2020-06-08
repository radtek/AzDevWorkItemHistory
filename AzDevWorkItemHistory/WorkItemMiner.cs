using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace WorkItemHistory
{
    public class WorkItemMiner
    {
        private readonly WorkItemTrackingHttpClient _client;
        private const int AzureDevOpsWorkItemBatchSize = 200;

        public WorkItemMiner(string username, string pat, Uri azureUri)
        {
            _client = new WorkItemTrackingHttpClient(azureUri, new VssBasicCredential(username, pat));
        }

        public async IAsyncEnumerable<IDictionary<string, object>> ExecuteQueryAsync(Guid queryId)
        {
            var queryResult = await _client.QueryByIdAsync(queryId);

            foreach (var batched in queryResult.WorkItems.Batch(AzureDevOpsWorkItemBatchSize))
            {
                var batchResult = await _client.GetWorkItemsBatchAsync(new WorkItemBatchGetRequest
                {
                    Ids = batched.Select(i => i.Id),
                    Fields = queryResult.Columns.Select(c => c.ReferenceName)
                });

                foreach (var item in batchResult)
                {
                    yield return queryResult.Columns.ToDictionary(
                        fieldRef => fieldRef.Name,
                        fieldRef => item.Fields.ContainsKey(fieldRef.ReferenceName)
                            ? item.Fields[fieldRef.ReferenceName]
                            : "");
                }
            }
        }

        public async IAsyncEnumerable<WorkItem> GetAllWorkItemRevisionsForProjectAsync(string projectName)
        {
            string continuationToken = null;
            ReportingWorkItemRevisionsBatch revisions;
            do
            {
                revisions = await _client.ReadReportingRevisionsGetAsync(projectName, continuationToken: continuationToken);
                continuationToken = revisions.ContinuationToken;

                foreach (var item in revisions.Values)
                    yield return item;

            } while (!revisions.IsLastBatch);
        }

        public async Task<IEnumerable<(WorkItemType workItemType, IEnumerable<WorkItemStateColor> workItemCategory)>> GetWorkItemTypeStates(string projectName)
        {
            return Evaluate(await GetWorkItemTypeStates());

            async Task<IEnumerable<(WorkItemType workItemType, IEnumerable<WorkItemStateColor> workItemCategory)>> GetWorkItemTypeStates()
            {
                return (await _client.GetWorkItemTypesAsync(projectName))
                   .Select(async t => (t, (await _client.GetWorkItemTypeStatesAsync(projectName, t.Name)).AsEnumerable()))
                   .Select(x => x.Result);
            }

            static IEnumerable<(WorkItemType workItemType, IEnumerable<WorkItemStateColor> workItemCategory)> Evaluate(IEnumerable<(WorkItemType workItemType, IEnumerable<WorkItemStateColor> workItemCategory)> states)
            {
                return states
                   .ToList()
                   .Select(x => (x.workItemType, workItemCategory: x.workItemCategory.ToList().AsEnumerable()));
            }
        }
    }
}