# AzDevWorkItemHistory
Command Line tool for executing queries and inspecting history of Azure DevOps Work Item Tracking

[![Build Status](https://dev.azure.com/jon-fuller/AzDevWorkItemHistory/_apis/build/status/sep.AzDevWorkItemHistory?branchName=master)](https://dev.azure.com/jon-fuller/AzDevWorkItemHistory/_build/latest?definitionId=1&branchName=master)

## Connecting to Azure DevOps

`azureUri` - This is the root of your Azure DevOps url. e.g. `https://contoso.visualstudio.com`.

`username` - This is the username you use to log in to Azure DevOps.

`pat` - This is your **Personal Access Token**. More info here: https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops

## Installation

```
dotnet tool install -g azure-boards-workitems
```

## Usage


```
$ ./AzDevWorkItemHistory.exe --help                               AzDevWorkItemHistory 1.4.0
Copyright c 2019 SEP, Inc

  login        Stores encrypted credentials for Azure.

  logout       Removes a set of credentials for the given URI.

  query        Executes a query.

  revisions    Fetches all work item revisions for a project (useful for data-mining and reporting).

  durations    Fetches timespans of all work items for a project.

  all          Fetches all work items for a project.

  help         Display more information on a specific command.

  version      Display version information.
```

### Examples

Login to your Azure DevOps instance:

```
$ azure-boards-workitems login --azureUri=https://contoso.visualstudio.com/ --pat=generated-personal-access-token --username=alice
```

Viewing the results of a query whose GUID is `QUERY-GUID`:

```
azure-boards-workitems query --azureUri=https://contoso.visualstudio.com/ --queryId=this-is-a-query-guid
```

**CSV** - Create a CSV containing all revisions of all work items in project `PHOENIX`:

```
azure-boards-workitems all --azureUri=https://contoso.visualstudio.com/ --project=Phoenix > phoenix_revisions.csv
```