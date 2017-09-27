using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;

namespace EventCounterTest
{
    class Program
    {
        static void Main(string[] args) => AsyncMain(args).Wait();

        static async Task AsyncMain(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, a) =>
            {
                cts.Cancel();
                a.Cancel = true;
            };

            var listener = new HostingEventSourceListener();

            Console.WriteLine("Press Ctrl-C to stop");
            for (var i = 0; !cts.Token.IsCancellationRequested; i++)
            {
                var url = $"https://localhost/?iter={i}";

                MinimalEventCounterSource.Log.Request(url, i);
                try
                {
                    await Task.Delay(100, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }
    }

    [EventSource(Name = "Samples-EventCounterDemos-Minimal")]
    public sealed class MinimalEventCounterSource : EventSource
    {
        public static MinimalEventCounterSource Log = new MinimalEventCounterSource();
        private EventCounter _requestCounter;

        private MinimalEventCounterSource()
        {
            _requestCounter = new EventCounter("request", this);
        }

        /// <summary>
        /// Call this method to indicate that a request for a URL was made which tool a particular amount of time
        /// </summary>
        public void Request(string url, float elapsedMSec)
        {
            WriteEvent(1, url, elapsedMSec);    // This logs it to the event stream if events are on.    
            _requestCounter.WriteMetric(elapsedMSec);        // This adds it to the EventCounter called 'Request' if PerfCounters are on
        }
    }
}
