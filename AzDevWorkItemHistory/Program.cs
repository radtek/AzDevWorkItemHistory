using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;

namespace WorkItemHistory
{
    public class ExitCode
    {
        public static ExitCode Success = new ExitCode(0, string.Empty);
        public static ExitCode GeneralError = new ExitCode(-1, "unspecified error");
        public static ExitCode DuplicateUri(string uri) => new ExitCode(2, "This URI () already exists. You need to 'logout' with that URI if you wish to update the credentials for that URI.");

        private ExitCode(int value, string message)
        {
            Value = value;
        }

        public int Value { get; }
        
    }
    class Program
    {
        static Task<ExitCode> Err(IEnumerable<Error> parseErrors)
        {
            return Task.FromResult(ExitCode.GeneralError);
        }

        static async Task<int> Main(string[] args)
        {
            var runner = new Runner(System.Console.Out, System.Console.Error);
            var result = Parser.Default.ParseArguments<QueryOptions, RevisionsOptions, DurationsOptions, AllWorkItemsOptions>(args)
                .MapResult(
                    (QueryOptions opts) => runner.RunQuery(opts),
                    (RevisionsOptions opts) => runner.RunRevisions(opts),
                    (AllWorkItemsOptions opts) => runner.AllWorkItems(opts),
                    (DurationsOptions opts) => runner.WorkItemDurations(opts),
                    Err);

            return (await result).Value;
        }
    }
}
