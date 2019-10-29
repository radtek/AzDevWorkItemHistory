using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzDevWorkItemHistory.Credentials;
using CommandLine;

namespace WorkItemHistory
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var stdOut = Console.Out;
            var stdErr = Console.Error;
            var credentialManager = new CredentialManager(new YamlFileCredentialStore("credentials.yml"));

            var runner =
                new CheckLoginStatusRunner(credentialManager,    // ensures the the user is logged in for this URI
                new AuthorizedRunner(                            // try/catch for the actual operation, looks for unauthorized exception
                new Runner(stdOut, stdErr, credentialManager))); // actually does the thing

            var result = Parser.Default.ParseArguments<LoginOptions, QueryOptions, RevisionsOptions, DurationsOptions, AllWorkItemsOptions>(args)
                .MapResult(
                    (LoginOptions opts) => runner.Login(opts),
                    (QueryOptions opts) => runner.RunQuery(opts),
                    (RevisionsOptions opts) => runner.RunRevisions(opts),
                    (AllWorkItemsOptions opts) => runner.AllWorkItems(opts),
                    (DurationsOptions opts) => runner.WorkItemDurations(opts),
                    Err);

            var exitCode = await result;
            stdErr.WriteLine($"{exitCode.Value} - {exitCode.Message}");
            return exitCode.Value;
        }
        static Task<ExitCode> Err(IEnumerable<Error> parseErrors)
        {
            return Task.FromResult(ExitCode.GeneralError);
        }
    }
}
