# AzDevWorkItemHistory
Command Line tool for executing queries and inspecting history of Azure DevOps Work Item Tracking

[![Build Status](https://dev.azure.com/jon-fuller/AzDevWorkItemHistory/_apis/build/status/sep.AzDevWorkItemHistory?branchName=master)](https://dev.azure.com/jon-fuller/AzDevWorkItemHistory/_build/latest?definitionId=1&branchName=master)

## Connecting to Azure DevOps

`azureUri` - This is the root of your Azure DevOps url. e.g. `https://contoso.visualstudio.com`.

`username` - This is the username you use to log in to Azure DevOps.

`pat` - This is your **Personal Access Token**. More info here: https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops


## Usage

```
$ ./AzDevWorkItemHistory.exe --help                                                                                                                                          AzDevWorkItemHistory 1.0.0                                                                                                                                                   Copyright (C) 2019 AzDevWorkItemHistory

  query        Executes a query.

  revisions    Fetches all work item revisions for a project.

  durations    Fetches timespans of all work items for a project.

  help         Display more information on a specific command.

  version      Display version information.
```

### Examples

Viewing the results of a query whose GUID is `QUERY-GUID`:

```
$ ./AzDevWorkItemHistory.exe query \
--username="your_username@acme.org" \
--pat="YOUR_SECRET_TOKEN_HERE" \
--azureUri="https://contoso.visualstudio.com" \
--queryId="QUERY-GUID"
```

**CSV** - Create a CSV containing all revisions of all work items in project `PHOENIX`:

```
$ ./AzDevWorkItemHistory.exe revisions \
--username="your_username@acme.org" \
--pat="YOUR_SECRET_TOKEN_HERE" \
--azureUri="https://contoso.visualstudio.com" \
--project="PHOENIX" > phoenix_revisions.csv
```