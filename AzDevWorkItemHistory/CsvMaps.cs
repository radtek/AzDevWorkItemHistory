using System;
using CsvHelper.Configuration;
using LanguageExt;

namespace WorkItemHistory
{
    public class CsvMaps
    {
        public class WorkItemInfoMap : ClassMap<WorkItemInfo>
        {
            public WorkItemInfoMap()
            {
                AutoMap(new Configuration {MemberTypes = MemberTypes.Fields});
                Map(m => m.Start).ConvertUsing(d => DateTimeString(d.Start));
                Map(m => m.End).ConvertUsing(d => DateTimeString(d.Start));
            }
            string DateTimeString(Option<DateTime> item)
            {
                return item.Map(d => d.ToString()).IfNone(string.Empty);
            }
        }
    }
}