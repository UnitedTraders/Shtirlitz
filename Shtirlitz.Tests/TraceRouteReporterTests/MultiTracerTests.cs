using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using Shtirlitz.Reporter.TraceRouteReporter;
using Shtirlitz.Reporter.TraceRouteReporter.Trace;
using Xunit;

namespace Shtirlitz.Tests.TraceRouteReporterTests
{
    public class MultiTracerTests
    {
        private readonly MultiTracer _multiTracer;

        public MultiTracerTests()
        {
            _multiTracer = new MultiTracer(new TestTracer());
        }

        [Fact]
        public void ResultsCount()
        {
            const int addressCount = 20;
            var addreses = new List<string> ();
            for (var i = 0; i < addressCount; i++)
            {
                addreses.Add(IPAddress.Loopback.ToString());
                 
            }

            var resultAggregator = new ConcurrentBag<string>();
            // act
            _multiTracer.Trace(addreses, resultAggregator.Add , CancellationToken.None);

            
            // assert
            Assert.True(resultAggregator.Count == addressCount+1);
        }




        private class TestTracer : ITracer
        {
            public IEnumerable<TracertEntry> Tracert(string hostNameOrAddress, int maxHops, int timeout, int packageSize)
            {
                var res = new List<TracertEntry> {
                    new TracertEntry
                {
                    Address = IPAddress.Loopback,
                        ReplyStatus = IPStatus.Success,
                        Ttl = 1,
                        RoundTrip = 1,
                },
                new TracertEntry
                {
                     Address = IPAddress.None,
                    ReplyStatus = IPStatus.Success,
                    Ttl = 2,
                    RoundTrip = 2,
                }
                };
                return res;
            }

            public PingEntry Ping(string hostNameOrAddress, int timeout)
            {
                var ip = IPAddress.Parse(hostNameOrAddress);
                return new PingEntry
                {
                    Address = ip,
                    ReplyStatus = IPStatus.Success,
                    RoundTrip = 10
                };
            }

            public void GetHostNameAsync(IPAddress address, Action<IPAddress, string> result)
            {
                if (address.Equals(IPAddress.Loopback))
                    result(address, "TestTracer");

            }
        }

    }
}