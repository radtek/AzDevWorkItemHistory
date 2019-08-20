using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;

namespace WorkItemHistory
{
    class Program
    {
        static Task<int> Err(IEnumerable<Error> parseErrors)
        {
            return Task.FromResult(-1);
        }

        static async Task<int> Main(string[] args)
        {
            var runner = new Runner(System.Console.Out, System.Console.Error);
            var result = Parser.Default.ParseArguments<QueryOptions, RevisionsOptions, DurationsOptions>(args)
                .MapResult(
                    (QueryOptions opts) => runner.RunQuery(opts),
                    (RevisionsOptions opts) => runner.RunRevisions(opts),
                    (DurationsOptions opts) => runner.WorkItemDurations(opts),
                    Err);

            return await result;
        }
    }
}
