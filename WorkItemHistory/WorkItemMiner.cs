using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace WorkItemHistory
{
    public class WorkItemMiner
    {
        private readonly WorkItemTrackingHttpClient _client;

        public WorkItemMiner(string username, string pat, Uri azureUri)
        {
            _client = new WorkItemTrackingHttpClient(azureUri, new VssBasicCredential(username, pat));
        }

        public async Task<IEnumerable<WorkItemReference>> ExecuteQueryAsync(Guid queryId)
        {
            return (await _client.QueryByIdAsync(queryId)).WorkItems;
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