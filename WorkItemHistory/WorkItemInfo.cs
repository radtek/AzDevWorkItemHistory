using System;
using LanguageExt;

namespace WorkItemHistory
{
    public class WorkItemInfo : Record<WorkItemInfo>
    {
        public readonly string Title;
        public readonly string WorkItemType;
        public readonly string IterationPath;
        public readonly string State;
        public readonly string AreaPath;
        public readonly string TeamProject;
        public readonly int Id;
        public readonly Option<DateTime> Start;
        public readonly Option<DateTime> End;

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