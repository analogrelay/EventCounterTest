using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace EventCounterTest
{
    public class HostingEventSourceListener : EventListener
    {
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name.Equals("Samples-EventCounterDemos-Minimal"))
            {
                EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All, new Dictionary<string, string>()
                {
                    { "EventCounterIntervalSec", "1" }
                });
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            Console.WriteLine($"{eventData.EventName}");
            foreach (var payload in eventData.Payload)
            {
                if (payload is IDictionary<string, object> payloadDictionary)
                {
                    foreach (var data in payloadDictionary)
                    {
                        Console.WriteLine($" {data.Key} - {data.Value}");
                    }
                }
            }
        }
    }
}
