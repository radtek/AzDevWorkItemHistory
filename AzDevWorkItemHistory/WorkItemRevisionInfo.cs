using System;
using LanguageExt;

namespace WorkItemHistory
{
    public class WorkItemRevisionInfo : Record<WorkItemRevisionInfo>
    {
        public readonly string Title;
        public readonly string WorkItemType;
        public readonly string IterationPath;
        public readonly string State;
        public readonly string AreaPath;
        public readonly string TeamProject;
        public readonly int Id;
        public readonly int Revision;
        public readonly DateTime ChangeDate;

        public WorkItemRevisionInfo(string title, string workItemType, string iterationPath, string state,
            string areaPath, string teamProject, int id, int revision, DateTime changeDate)
        {
            Title = title;
            WorkItemType = workItemType;
            IterationPath = iterationPath;
            State = state;
            AreaPath = areaPath;
            TeamProject = teamProject;
            Id = id;
            Revision = revision;
            ChangeDate = changeDate;
        }
    }
}