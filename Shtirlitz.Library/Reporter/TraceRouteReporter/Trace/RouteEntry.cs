using System.Net;

namespace Shtirlitz.Reporter.TraceRouteReporter.Trace
{

    //Format class from http://winmtrnet.codeplex.com/
    internal class RouteEntry
    {
        public IPAddress Address { get; set; }
        public bool IsNull { get; set; }
        public int Ttl { get; set; }
        public string Text { get; set; }
        public long LastRoundTrip { get; set; }
        public long AvgRoundTrip { get; set; }
        public long BestRoundTrip { get; set; }
        public long WorstRoundTrip { get; set; }
        public long SentPings { get; set; }
        public long RecvPings { get; set; }
        public float Loss { get; set; }
        public string HostName { get; set; }

        public RouteEntry()
        {
            BestRoundTrip = long.MaxValue;
            WorstRoundTrip = 0;
        }
    }
}