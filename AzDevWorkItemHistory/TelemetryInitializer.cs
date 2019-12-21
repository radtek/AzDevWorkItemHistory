using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace WorkItemHistory
{
    public class TelemetryInitializer : ITelemetryInitializer
    {
        private readonly Action<ITelemetry> _init;

        public static ITelemetryInitializer Create(Action<ITelemetry> init)
        {
            return new TelemetryInitializer(init);
        }

        private TelemetryInitializer(Action<ITelemetry> init)
        {
            _init = init;
        }

        public void Initialize(ITelemetry telemetry)
        {
            _init(telemetry);
        }
    }
 }
