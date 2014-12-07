using System;
using System.Collections.Generic;
using System.Net;

namespace Shtirlitz.Reporter.TraceRouteReporter.Trace
{
    public interface ITracer
    {
        IEnumerable<TracertEntry> Tracert(string hostNameOrAddress, int maxHops, int timeout, int packageSize);
        PingEntry Ping(string hostNameOrAddress, int timeout);
        void GetHostNameAsync(IPAddress address, Action<IPAddress, string> result);
    }
}