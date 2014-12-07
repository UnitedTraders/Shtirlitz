using System.Net;
using System.Net.NetworkInformation;

namespace Shtirlitz.Reporter.TraceRouteReporter.Trace
{
    public class TracertEntry: PingEntry
    {
        /// <summary>
        /// The hop id. Represents the number of the hop.
        /// </summary>
        public int Ttl { get; set; }
    }


    public class PingEntry
    {
        public IPAddress Address { get; set; }
        public long RoundTrip { get; set; }
        public IPStatus ReplyStatus { get; set; }
    }

   
}
