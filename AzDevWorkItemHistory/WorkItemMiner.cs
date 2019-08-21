using System;
using System.Collections.Generic;
using System.Linq;
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

        public async IAsyncEnumerable<WorkItem> ExecuteQueryAsync(Guid queryId)
        {
            var queryResult = await _client.QueryByIdAsync(queryId);

            foreach (var batched in queryResult.WorkItems.Batch(AzureDevOpsWorkItemBatchSize))
            {
                var batchResult = await _client.GetWorkItemsBatchAsync(new WorkItemBatchGetRequest { Ids = batched.Select(i => i.Id) });

                foreach (var item in batchResult)
                    yield return item;
            }
        }

        public async IAsyncEnumerable<WorkItem> GetAllWorkItemRevisionsForProjectAsync(string projectName)
        {
            var revisions = await _client.ReadReportingRevisionsGetAsync(projectName);
            while (!revisions.IsLastBatch)
            {
                foreach (var item in revisions.Values)
                    yield return item;

                revisions = await _client.ReadReportingRevisionsGetAsync(projectName, continuationToken: revisions.ContinuationToken);
            }
        }
    }
}