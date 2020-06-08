using System;
using LanguageExt;

namespace WorkItemHistory
{
    public class WorkItemRevisionInfo : Record<WorkItemRevisionInfo>, IWorkItemInfo
    {
        public string Title { get; }
        public string WorkItemType { get; }
        public string IterationPath { get; }
        public string State { get; }
        public string StateCategory { get; }
        public string AreaPath { get; }
        public string TeamProject { get; }
        public int Id { get; }
        public int Revision { get; }
        public DateTime ChangeDate { get; }

        public WorkItemRevisionInfo(string title, string workItemType, string iterationPath, string state,
            string stateCategory, string areaPath, string teamProject, int id, int revision, DateTime changeDate)
        {
            Title = title;
            WorkItemType = workItemType;
            IterationPath = iterationPath;
            State = state;
            StateCategory = stateCategory;
            AreaPath = areaPath;
            TeamProject = teamProject;
            Id = id;
            Revision = revision;
            ChangeDate = changeDate;
        }
    }
}