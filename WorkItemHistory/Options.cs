using System;
using CommandLine;

namespace WorkItemHistory
{
    [Verb("query", HelpText = "Executes a query.")]
    public class QueryOptions : Options
    {

        [Option(longName: "queryId", HelpText = "The GUID of the query.", Required = true, SetName = QueryVsRevisionsSet)]
        public string QueryId { get; set; }

        public Guid GetQueryId()
        {
            return Guid.Parse(QueryId);
        }
    }

    public class ProjectOptions : Options
    {
        [Option(longName: "project", HelpText = "The project name to query revisions for.", SetName = QueryVsRevisionsSet)]
        public string Project { get; set; }
    }

    [Verb("revisions", HelpText = "Fetches all work item revisions for a project.")]
    public class RevisionsOptions : ProjectOptions
    {
    }

    [Verb("durations", HelpText = "Fetches timespans of all work items for a project.")]
    public class DurationsOptions : ProjectOptions
    {
    }

    public class Options
    {
        protected const string QueryVsRevisionsSet = "QueryVsRevisionsSet";
        //var username = "fullerjc@gmail.com";
        //var pat = "knmspqj5q4q7buj4vpqu5u2momwmwbuhf2aabz4l6btjiqh3wqba";
        // "https://newmill.visualstudio.com"
        //"041fb898-5d65-439e-992a-273157a3d843"

        [Option(longName: "username", HelpText = "Azure DevOps username", Required = true)]
        public string Username { get; set; }

        [Option(longName: "pat", HelpText = "Personal Access Token (https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops#create-personal-access-tokens-to-authenticate-access)", Required = true)]
        public string PersonalAccessToken { get; set; }

        [Option(longName: "azureUri", HelpText = "The base URI of your Azure DevOps instance. (e.g. https://contoso.visualstudio.com)", Required = true)]
        public string AzureUri { get; set; }

        public Uri GetAzureUri()
        {
            return new Uri(AzureUri);
        }
    }
}