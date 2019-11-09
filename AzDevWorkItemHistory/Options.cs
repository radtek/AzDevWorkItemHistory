using System;
using System.Collections.Generic;
using CommandLine;

namespace WorkItemHistory
{
    [Verb("query", HelpText = "Executes a query.")]
    public partial class QueryOptions : AzureOptions
    {
        [Option(longName: "queryId", HelpText = "The GUID of the query.", Required = true)]
        [NoAnalytics]
        public string QueryId { get; set; }

        public Guid GetQueryId()
        {
            return Guid.Parse(QueryId);
        }
    }

    [Verb("revisions", HelpText = "Fetches all work item revisions for a project (useful for data-mining and reporting).")]
    public partial class RevisionsOptions : ProjectOptions
    {
    }

    [Verb("all", HelpText = "Fetches all work items for a project.")]
    public partial class AllWorkItemsOptions : ProjectOptions
    {
    }

    [Verb("durations", HelpText = "Fetches timespans of all work items for a project.")]
    public partial class DurationsOptions : ProjectOptions
    {
    }

    [Verb("logout", HelpText = "Removes a set of credentials for the given URI.")]
    public partial class LogoutOptions : AzureOptions
    {
    }

    [Verb("login", HelpText = "Stores encrypted credentials for Azure.")]
    public partial class LoginOptions : AzureOptions
    {
        [Option(longName: "username", HelpText = "Azure DevOps username", Required = true)]
        [NoAnalytics]
        public string Username { get; set; }

        [Option(longName: "pat", HelpText = "Personal Access Token (https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops#create-personal-access-tokens-to-authenticate-access)", Required = true)]
        [NoAnalytics]
        public string PersonalAccessToken { get; set; }
    }

    public partial class ProjectOptions : AzureOptions
    {
        [Option(longName: "project", HelpText = "The project name to query revisions for.", Required = true)]
        public string Project { get; set; }

        [Option(longName: "areaPath", HelpText = "Filter work items by the specified area path. (can specify multiple)", Required = false, Separator = ',')]
        public IEnumerable<string> AreaPath { get; set; }

        [Option(longName: "iterationPath", HelpText = "Filter work items by the specified iteration path. (can specify multiple)", Required = false, Separator = ',')]
        public IEnumerable<string> IterationPath { get; set; }

        [Option(longName: "state", HelpText = "Filter work items by the specified state. (can specify multiple)", Required = false, Separator = ',')]
        public IEnumerable<string> State { get; set; }

        [Option(longName: "type", HelpText = "Filter work items by the specified work item type. (can specify multiple)", Required = false, Separator = ',')]
        public IEnumerable<string> Type { get; set; }
    }

    public class AzureOptions
    {
        protected const string ExampleUri = "https://contoso.visualstudio.com";

        [Option(longName: "azureUri", HelpText = "The base URI of your Azure DevOps instance. (e.g. " + ExampleUri + ")", Required = true)]
        [NoAnalytics]
        public Uri AzureUri { get; set; }
    }
}