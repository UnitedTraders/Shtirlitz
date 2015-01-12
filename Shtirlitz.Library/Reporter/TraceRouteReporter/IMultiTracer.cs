using System;
using System.Collections.Generic;
using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Reporter.TraceRouteReporter
{
    public interface IMultiTracer
    {
        int MaxHops { get; set; }
        int PingTimeout { get; set; }
        int PingTimeFrame { get; set; }
        int PackageSize { get; set; }
        int NumberOfRounds { get; set; }
        void Trace(IEnumerable<string> addresslist, Action<string> printResults, CancellationToken token, SimpleProgressCallback progressCallback =null);
    }
}