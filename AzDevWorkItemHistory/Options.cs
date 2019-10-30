using System;
using System.Collections.Generic;
using CommandLine;

namespace WorkItemHistory
{
    [Verb("query", HelpText = "Executes a query.")]
    public class QueryOptions : ProjectOptions
    {
        [Option(longName: "queryId", HelpText = "The GUID of the query.", Required = true)]
        public string QueryId { get; set; }

        public Guid GetQueryId()
        {
            return Guid.Parse(QueryId);
        }
    }

    public class ProjectOptions
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

        [Option(longName: "azureUri", HelpText = "The base URI of your Azure DevOps instance. (e.g. https://contoso.visualstudio.com)", Required = true)]
        public Uri AzureUri { get; set; }
    }

    [Verb("revisions", HelpText = "Fetches all work item revisions for a project.")]
    public class RevisionsOptions : ProjectOptions
    {
    }

    [Verb("all", HelpText = "Fetches all work items for a project.")]
    public class AllWorkItemsOptions : ProjectOptions
    {
    }

    [Verb("durations", HelpText = "Fetches timespans of all work items for a project.")]
    public class DurationsOptions : ProjectOptions
    {
    }

    [Verb("logout", HelpText = "Removes a set of credentials for the given URI.")]
    public class LogoutOptions
    {
        [Option(longName: "azureUri", HelpText = "The base URI of your Azure DevOps instance. (e.g. https://contoso.visualstudio.com)", Required = true)]
        public Uri AzureUri { get; set; }
    }

    [Verb("login", HelpText = "Stores encrypted credentials for Azure.")]
    public class LoginOptions
    {
        [Option(longName: "username", HelpText = "Azure DevOps username", Required = true)]
        public string Username { get; set; }

        [Option(longName: "pat", HelpText = "Personal Access Token (https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops#create-personal-access-tokens-to-authenticate-access)", Required = true)]
        public string PersonalAccessToken { get; set; }

        [Option(longName: "azureUri", HelpText = "The base URI of your Azure DevOps instance. (e.g. https://contoso.visualstudio.com)", Required = true)]
        public Uri AzureUri { get; set; }
    }
}