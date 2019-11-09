using CommandLine;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WorkItemHistory
{
    public static class AnalyticsHelpers
    {
        public static string GetVerb<TOpts>()
        {
            return typeof(TOpts)
                .GetCustomAttributes(typeof(VerbAttribute), true)
                .Take(1)
                .Cast<VerbAttribute>()
                .First()
                .Name;
        }

        public static Dictionary<string, string> GetEventData<TOpts>(TOpts opts)
        {
            return typeof(TOpts)
                .GetProperties()
                .Select(prop => (prop: prop.GetValue(opts)?.ToString(), option: GetOption(prop)))
                .Where(p => p.prop != null)
                .Where(p => p.option.IsSome)
                .ToDictionary(p => GetLongName(p.option), p => p.prop);

            static string GetLongName(Option<OptionAttribute> opt)
            {
                return opt.Match(
                    Some: x => x.LongName,
                    None: "whatever");
            }
            static Option<OptionAttribute> GetOption(PropertyInfo prop)
            {
                if (prop.GetCustomAttributes<NoAnalyticsAttribute>().Any())
                    return Option<OptionAttribute>.None;

                return prop.GetCustomAttributes<OptionAttribute>().FirstOrDefault()
                    ?? Option<OptionAttribute>.None;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NoAnalyticsAttribute : Attribute
    {
    }
}