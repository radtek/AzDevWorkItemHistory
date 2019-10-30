using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace WorkItemHistory
{
    public partial class QueryOptions
    {
        [Usage(ApplicationAlias = Program.ApplicationExecutableName)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Execute a query",
                    UnParserSettings.WithUseEqualTokenOnly(),
                    new QueryOptions
                    {
                        AzureUri = new Uri(ExampleUri),
                        QueryId = "this-is-a-query-guid",
                    });
            }
        }
    }

    public partial class RevisionsOptions
    {
        [Usage(ApplicationAlias = Program.ApplicationExecutableName)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Get all revisions for all work items.",
                    UnParserSettings.WithUseEqualTokenOnly(),
                    new RevisionsOptions
                    {
                        AzureUri = new Uri(ExampleUri),
                        Project = "Phoenix",
                    });
                yield return new Example(
                    "Get all revisions for User Story and Bug types.",
                    UnParserSettings.WithUseEqualTokenOnly(),
                    new RevisionsOptions
                    {
                        AzureUri = new Uri(ExampleUri),
                        Project = "Phoenix",
                        Type = new[] { "User Story", "Bug" }
                    });
                yield return new Example(
                    "Get all revisions for the 'UI' and 'Dev Ops' Area Paths.",
                    UnParserSettings.WithUseEqualTokenOnly(),
                    new RevisionsOptions
                    {
                        AzureUri = new Uri(ExampleUri),
                        Project = "Phoenix",
                        AreaPath = new[] { "UI", "Dev Ops" }
                    });
            }
        }
    }

    public partial class AllWorkItemsOptions
    {
        [Usage(ApplicationAlias = Program.ApplicationExecutableName)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Get all work items for a project.",
                    UnParserSettings.WithUseEqualTokenOnly(),
                    new AllWorkItemsOptions
                    {
                        AzureUri = new Uri(ExampleUri),
                        Project = "Phoenix",
                    });
                yield return new Example(
                    "Get all work items for User Story and Bug types.",
                    UnParserSettings.WithUseEqualTokenOnly(),
                    new AllWorkItemsOptions
                    {
                        AzureUri = new Uri(ExampleUri),
                        Project = "Phoenix",
                        Type = new[] { "User Story", "Bug" }
                    });
            }
        }
    }

    public partial class DurationsOptions
    {
        [Usage(ApplicationAlias = Program.ApplicationExecutableName)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Gets the durations for the work items in 'PI 16/Iteration 2'.",
                    UnParserSettings.WithUseEqualTokenOnly(),
                    new DurationsOptions
                    {
                        AzureUri = new Uri(ExampleUri),
                        Project = "Phoenix",
                        IterationPath = new[] { "PI 16/Iteration 2" }
                    });
            }
        }
    }

    public partial class LoginOptions
    {
        [Usage(ApplicationAlias = Program.ApplicationExecutableName)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Log in (cache credentials) for the given Azure DevOps instance.",
                    UnParserSettings.WithUseEqualTokenOnly(),
                    new LoginOptions()
                    {
                        AzureUri = new Uri(ExampleUri),
                        Username = "alice",
                        PersonalAccessToken = "generated-personal-access-token"
                    });
            }
        }
    }

    public partial class LogoutOptions
    {
        [Usage(ApplicationAlias = Program.ApplicationExecutableName)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Log out (remove cached credentials) for the given Azure DevOps instance.",
                    UnParserSettings.WithUseEqualTokenOnly(),
                    new LogoutOptions
                    {
                        AzureUri = new Uri(ExampleUri),
                    });
            }
        }
    }
}
