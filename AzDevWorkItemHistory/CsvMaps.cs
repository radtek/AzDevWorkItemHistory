using System;
using System.Globalization;
using CsvHelper.Configuration;
using LanguageExt;

namespace WorkItemHistory
{
    public static class CsvMaps
    {
        public sealed class WorkItemInfoMap : ClassMap<WorkItemInfo>
        {
            public WorkItemInfoMap()
            {
                AutoMap(new Configuration {MemberTypes = MemberTypes.Fields});
                Map(m => m.Start).Index(1).ConvertUsing(d => DateTimeString(d.Start));
                Map(m => m.End).Index(0).ConvertUsing(d => DateTimeString(d.Start));
                Map(m => m.WorkItemType).Index(2);
                Map(m => m.Id).Index(3);
                Map(m => m.Title).Index(4);
            }
            string DateTimeString(Option<DateTime> item)
            {
                return item.Map(d => d.ToString(CultureInfo.CurrentCulture)).IfNone(string.Empty);
            }
        }
    }
}