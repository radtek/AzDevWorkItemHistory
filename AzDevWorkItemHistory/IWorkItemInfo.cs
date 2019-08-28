namespace WorkItemHistory
{
    public interface IWorkItemInfo
    {
        string AreaPath { get; }
        string IterationPath { get; }
        string WorkItemType { get; }
        string State { get; }
    }
}