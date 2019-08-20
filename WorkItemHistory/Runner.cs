using System.IO;
using System.Threading.Tasks;

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

            await foreach (var workItem in revisions)
            {
                _stdout.WriteLine($"{workItem.Id} - {workItem.Rev}");
            }

            return 0;
        }
    }
}