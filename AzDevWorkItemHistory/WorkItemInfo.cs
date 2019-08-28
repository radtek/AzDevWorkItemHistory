using System;
using LanguageExt;

namespace WorkItemHistory
{
    public class WorkItemInfo : Record<WorkItemInfo>, IWorkItemInfo
    {
        public string Title { get; }
        public string WorkItemType { get; }
        public string IterationPath { get; }
        public string State { get; }
        public string AreaPath { get; }
        public string TeamProject { get; }
        public int Id { get; }
        public Option<DateTime> Start { get; }
        public Option<DateTime> End { get; }

        public WorkItemInfo(string title, string workItemType, string iterationPath, string state,
            string areaPath, string teamProject, int id, Option<DateTime> start, Option<DateTime> end)
        {
            Title = title;
            WorkItemType = workItemType;
            IterationPath = iterationPath;
            State = state;
            AreaPath = areaPath;
            TeamProject = teamProject;
            Id = id;
            Start = start;
            End = end;
        }
    }
}