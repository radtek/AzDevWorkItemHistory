using CommandLine;
using NUnit.Framework;
using WorkItemHistory;

namespace Tests
{
    public class AnalyticsHelpersTests
    {
        [Test]
        public void GetsTheVerbOfAnOptionsClass()
        {
            Assert.That(AnalyticsHelpers.GetVerb<OptsWithVerb>(), Is.EqualTo("TheVerb"));
        }

        [Test]
        public void GetsTheOptionsAsADictionary()
        {
            var opts = AnalyticsHelpers.GetEventData(new OptsWithVerb
            {
                FirstOption = "hello",
                SecondFlag = true,
                OptionNameWithSpaces = "world",
                PropWithNoOption = "goodbye"
            });

            Assert.That(opts, Contains.Key("first-option"));
            Assert.That(opts, Contains.Key("just-a-flag"));
            Assert.That(opts, Contains.Key("option name with spaces"));
            Assert.That(opts, Does.Not.ContainKey("PropWithNoOption"));

            Assert.That(opts["first-option"], Is.EqualTo("hello"));
            Assert.That(opts["just-a-flag"], Is.EqualTo("True"));
            Assert.That(opts["option name with spaces"], Is.EqualTo("world"));
        }

        [Test]
        public void DoesntGetOptionsWithNoAnalyticsAttribute()
        {
            var opts = AnalyticsHelpers.GetEventData(new OptsWithNoAnalyticsProp
            {
                AnOption = "hello",
            });

            Assert.That(opts, Does.Not.ContainKey("dont track me"));
        }

        [Test]
        public void DoesntGetOptionsWithoutValues()
        {
            var opts = AnalyticsHelpers.GetEventData(new OptsWithVerb
            {
                FirstOption = null,
            });

            Assert.That(opts, Does.Not.ContainKey("first-option"));
        }

        [Verb("AnotherVerb")]
        class OptsWithNoAnalyticsProp
        {
            [Option(longName: "dont track me")]
            [NoAnalytics]
            public string AnOption { get; set; }

        }
        [Verb("TheVerb")]
        class OptsWithVerb
        {
            [Option(longName: "first-option")]
            public string FirstOption { get; set; }

            [Option(longName: "just-a-flag")]
            public bool SecondFlag { get; set; }

            [Option(longName: "option name with spaces")]
            public string OptionNameWithSpaces { get; set; }

            public string PropWithNoOption { get; set; }
        }
    }
}