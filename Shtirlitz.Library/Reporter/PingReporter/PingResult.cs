
namespace Shtirlitz.Reporter.PingReporter
{
    public class PingResult
    {
        public string Host { get; set; }
        public int Send { get; set; }
        public int Receive { get; set; }
        public int Lost { get; set; }
        public long RoundtripTime { get; set; }
    }
}