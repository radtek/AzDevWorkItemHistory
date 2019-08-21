# AzDevWorkItemHistory
Command Line tool for executing queries and inspecting history of Azure DevOps Work Item Tracking

[![Build status](https://ci.appveyor.com/api/projects/status/8tkfs728t9s49rkv?svg=true)](https://ci.appveyor.com/project/jonfuller/azdevworkitemhistory)

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
