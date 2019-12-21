using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzDevWorkItemHistory.Credentials;
using CommandLine;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Security.Cryptography;

namespace WorkItemHistory
{
    class Program
    {
        public const string ApplicationExecutableName = "azure-boards-workitems";
        public const string InstrumentationKey = "1261066e-1707-4607-a992-80cdd2f4140f";

        static async Task<int> Main(string[] args)
        {
            var serviceProvider = ConfigureContainer();
            var telemetry = serviceProvider.GetRequiredService<TelemetryClient>();

            try
            {
                var stdOut = Console.Out;
                var stdErr = Console.Error;
                var path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ApplicationExecutableName,
                    "credentials.yml");
                var credentialManager = new CredentialManager(new YamlFileCredentialStore(path));
                var runner =
                    new AnalyticsRunner(telemetry,
                    new CheckLoginStatusRunner(credentialManager,    // ensures the the user is logged in for this URI
                    new AuthorizedRunner(                            // try/catch for the actual operation, looks for unauthorized exception
                    new Runner(stdOut, stdErr, credentialManager)))); // actually does the thing

                var result = Parser.Default.ParseArguments<LoginOptions, LogoutOptions, QueryOptions, RevisionsOptions, DurationsOptions, AllWorkItemsOptions>(args)
                    .MapResult(
                        (LoginOptions opts) => runner.Login(opts),
                        (LogoutOptions opts) => runner.Logout(opts),
                        (QueryOptions opts) => runner.RunQuery(opts),
                        (RevisionsOptions opts) => runner.RunRevisions(opts),
                        (AllWorkItemsOptions opts) => runner.AllWorkItems(opts),
                        (DurationsOptions opts) => runner.WorkItemDurations(opts),
                        (IEnumerable<Error> e) => Err(e, telemetry));

                var exitCode = await result;
                stdErr.WriteLine($"{exitCode.Value} - {exitCode.Message}");
                telemetry.Flush();
                return exitCode.Value;

            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
                throw;
            }
        }

        static Task<ExitCode> Err(IEnumerable<Error> parseErrors, TelemetryClient telemetry)
        {
            telemetry.TrackEvent("ParseError", GetTelemetryData());
            return Task.FromResult(ExitCode.ParseError);

            IDictionary<string, string> GetTelemetryData()
            {
                var errorData = parseErrors
                    .Select(e => (type: e.GetType(), message: e.ToString()))
                    .GroupBy(e => e.type)
                    .SelectMany(g => g.Select((e, i) => (key: $"{e.type.Name}-{i}", value: e.message)));

                return errorData
                    .Concat(new[] { (key: "NumErrors", value: parseErrors.Count().ToString()) })
                    .ToDictionary(x => x.key, x => x.value);
            }
        }

        private static IServiceProvider ConfigureContainer()
        {
            return new ServiceCollection()
               .AddSingleton<ITelemetryChannel>(new InMemoryChannel())
               .AddApplicationInsightsTelemetryWorkerService(opts =>
                {
                    opts.InstrumentationKey = InstrumentationKey;
                    opts.ApplicationVersion = typeof(Program).Assembly.GetName().Version.ToString();
                })
                .AddSingleton(typeof(ITelemetryInitializer), TelemetryInitializer.Create(x => {
                    x.Context.User.Id = GetUserId();
                    x.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
                }))
               .BuildServiceProvider();
        }

        private static string GetUserId()
        {
            return Hash($"{Environment.UserDomainName}-{Environment.UserName}");

            static string Hash(string input)
            {
                using var sha = new SHA1Managed();
                var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return HashToString(hashed);
            }

            static string HashToString(byte[] input)
            {
                var sb = new StringBuilder(input.Length * 2);

                foreach (byte b in input)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
