using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzDevWorkItemHistory.Credentials;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using LanguageExt;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using static LanguageExt.Prelude;

namespace WorkItemHistory
{
    public class Runner
    {
        private readonly TextWriter _stdout;
        private readonly TextWriter _stderr;
        private readonly CredentialManager _credentialManager;

        public Runner(TextWriter stdout, TextWriter stderr, CredentialManager credentialManager)
        {
            _stdout = stdout;
            _stderr = stderr;
            _credentialManager = credentialManager;
        }

        public Task<ExitCode> Login(LoginOptions options)
        {
            if (_credentialManager.HasUri(options.AzureUri))
                return Task.FromResult(ExitCode.DuplicateUri(options.AzureUri));

            _credentialManager.Add(CredentialV1.CreateFromPlainText(options.AzureUri, options.Username, options.PersonalAccessToken));
            _credentialManager.Save();

            return Task.FromResult(ExitCode.Success);
        }
        public Task<ExitCode> Logout(LogoutOptions opts)
        {
            _credentialManager.Remove(opts.AzureUri);
            _credentialManager.Save();

            return Task.FromResult(ExitCode.Success);
        }

        public async Task<ExitCode> RunQuery(QueryOptions options, CredentialV1 credentialV1)
        {
            var executor = new WorkItemMiner(credentialV1.Username, credentialV1.GetPlainTextPersonalAccessToken(), options.AzureUri);
            var items = executor.ExecuteQueryAsync(options.GetQueryId());

            CollectWorkItems(items).Result
                .ToList()
                .Then(WriteQueryResultCsv);

            return ExitCode.Success;
        }

        public async Task<ExitCode> AllWorkItems(AllWorkItemsOptions options, CredentialV1 credentialV1)
        {
            var executor = new WorkItemMiner(credentialV1.Username, credentialV1.GetPlainTextPersonalAccessToken(), options.AzureUri);
            var revisions = executor.GetAllWorkItemRevisionsForProjectAsync(options.Project);
            var workItems = CollectWorkItems(revisions).Result
                .Select(MapWorkItemRevision)
                .GroupBy(w => w.Id)
                .Select(g => g.OrderBy(x => x.ChangeDate).Last())
                .Then(items => ApplyFilter(items, options))
                .Select(w => new {w.Id, w.WorkItemType, w.Title})
                .ToList();

            WriteCsv(workItems);

            return ExitCode.Success;
        }

        public async Task<ExitCode> RunRevisions(RevisionsOptions options, CredentialV1 credentialV1)
        {
            var executor = new WorkItemMiner(credentialV1.Username, credentialV1.GetPlainTextPersonalAccessToken(), options.AzureUri);
            var revisions = executor.GetAllWorkItemRevisionsForProjectAsync(options.Project);
            var workItems = CollectWorkItems(revisions).Result
                .Select(MapWorkItemRevision)
                .Then(items => ApplyFilter(items, options))
                .ToList();

            WriteCsv(workItems);

            return ExitCode.Success;
        }

        public async Task<ExitCode> WorkItemDurations(DurationsOptions options, CredentialV1 credentialV1)
        {
            var executor = new WorkItemMiner(credentialV1.Username, credentialV1.GetPlainTextPersonalAccessToken(), options.AzureUri);
            var revisions = executor.GetAllWorkItemRevisionsForProjectAsync(options.Project);
            var workItems = CollectWorkItems(revisions).Result
                .Select(MapWorkItemRevision)
                .GroupBy(x => x.Id)
                .Select(ProcessWorkItem)
                .Then(items => ApplyFilter(items, options))
                .ToList();

            WriteCsv(workItems, cfg => cfg.RegisterClassMap<CsvMaps.WorkItemInfoMap>());

            return ExitCode.Success;
        }

        private IEnumerable<TWorkItemInfo> ApplyFilter<TWorkItemInfo>(IEnumerable<TWorkItemInfo> workItems, ProjectOptions options) where TWorkItemInfo : IWorkItemInfo
        {
            return workItems
                .Where(item => HasFilterOrMatches(item, x => x.IterationPath, x => x.IterationPath))
                .Where(item => HasFilterOrMatches(item, x => x.AreaPath, x => x.AreaPath))
                .Where(item => HasFilterOrMatches(item, x => x.State, x => x.State))
                .Where(item => HasFilterOrMatches(item, x => x.Type, x => x.WorkItemType));

            bool HasFilterOrMatches(TWorkItemInfo item, Func<ProjectOptions, IEnumerable<string>> specifiedFiltersSelector, Func<TWorkItemInfo, string> selector)
            {
                var specifiedFilters = specifiedFiltersSelector(options).ToList();
                var hasFilter = !specifiedFilters.Any();
                var target = selector(item);

                return hasFilter || specifiedFilters.Any(x => x.Equals(target, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        private void WriteQueryResultCsv(ICollection<IDictionary<string, object>> records)
        {
            if (records.Count != 0)
                WriteRecords(records.First().Keys, records.Select(r => r.Values));

            _stderr.WriteLine($"Saved {records.Count} items.");

            void WriteRecords(IEnumerable<string> header, IEnumerable<IEnumerable<object>> values)
            {
                using var csv = new CsvWriter(_stdout, leaveOpen: true, cultureInfo: CultureInfo.CurrentUICulture);
                csv.Configuration.TypeConverterCache.AddConverter<IdentityRef>(new IdentityRefConverter());

                WriteRecord(header, csv);
                foreach (var record in values)
                {
                    WriteRecord(record, csv);
                }
            }

            static void WriteRecord(IEnumerable<object> record, IWriter csv)
            {
                foreach (var field in record)
                {
                    csv.WriteField(field);
                }
                csv.NextRecord();
            }
        }
        private void WriteCsv<T>(ICollection<T> records, Action<IWriterConfiguration> config = null)
        {
            config ??= _ => {};

            using (var csv = new CsvWriter(_stdout, leaveOpen: true, cultureInfo: CultureInfo.CurrentUICulture))
            {
                config(csv.Configuration);
                csv.WriteRecords(records);

                _stderr.WriteLine($"Saved {records.Count} items.");
            }
        }

        private static async Task<IEnumerable<T>> CollectWorkItems<T>(IAsyncEnumerable<T> items)
        {
            var result = new List<T>();

            await foreach (var item in items)
            {
                result.Add(item);
            }

            return result;
        }

        private static WorkItemRevisionInfo MapWorkItemRevision(WorkItem item)
        {
            return new WorkItemRevisionInfo(
                title: item.Fields["System.Title"].ToString(),
                workItemType: item.Fields["System.WorkItemType"].ToString(),
                iterationPath: item.Fields["System.IterationPath"].ToString(),
                state: item.Fields["System.State"].ToString(),
                areaPath: item.Fields["System.AreaPath"].ToString(),
                teamProject: item.Fields["System.TeamProject"].ToString(),
                id: item.Id.Value,
                revision: item.Rev.Value,
                changeDate: DateTime.Parse(item.Fields["System.ChangedDate"].ToString()));
        }

        private static WorkItemInfo ProcessWorkItem(IEnumerable<WorkItemRevisionInfo> revisions)
        {
            var listed = revisions.ToList();
            var last = listed.Last();
            var (start, end) = GetWorkItemTimeRange(listed);

            return new WorkItemInfo(last.Title, last.WorkItemType, last.IterationPath, last.State, last.AreaPath, last.TeamProject, last.Id, start, end);
        }

        private static (Option<DateTime> start, Option<DateTime> end) GetWorkItemTimeRange(IEnumerable<WorkItemRevisionInfo> revisions)
        {
            Option<DateTime> activeDate = None;
            Option<DateTime> endDate = None;

            var lastState = string.Empty;

            foreach (var workItemVersion in revisions)
            {
                if ((lastState.Equals("New", StringComparison.OrdinalIgnoreCase) ||
                    lastState.Equals("Ready", StringComparison.OrdinalIgnoreCase) ||
                    lastState.Equals("UI Design", StringComparison.OrdinalIgnoreCase))
                    &&
                    workItemVersion.State.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    activeDate = Some(workItemVersion.ChangeDate);
                }

                if (!lastState.Equals("Closed", StringComparison.OrdinalIgnoreCase) &&
                    workItemVersion.State.Equals("Closed", StringComparison.OrdinalIgnoreCase))
                {
                    endDate = Some(workItemVersion.ChangeDate);
                }

                lastState = workItemVersion.State;
            }

            return (start: activeDate.IsSome ? activeDate : endDate, end: endDate);
        }

        private class IdentityRefConverter : ITypeConverter
        {
            public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return ((IdentityRef) value).DisplayName;
            }

            public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                throw new NotImplementedException();
            }
        }
    }
}