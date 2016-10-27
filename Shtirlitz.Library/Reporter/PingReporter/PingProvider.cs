using System.Net.NetworkInformation;

namespace Shtirlitz.Reporter.PingReporter
{
    public class PingProvider : IPingReporterProvider
    {
        public PingResult Current { get; private set; }

        public PingProvider(string host, int count)
        {
            _host = host;
            _count = count;
        }

        public bool Next()
        {
            var result = false;

            if (_count > 0)
            {
                result = SendPing(result);
                _count--;
            }

            return result;
        }

        private bool SendPing(bool result)
        {
            using (var ping = new Ping())
            {
                var pingResult = ping.Send(_host);
                _send++;
                if (pingResult != null)
                {
                    if (pingResult.Status == IPStatus.Success)
                        _receive++;

                    Current = new PingResult
                    {
                        Host = _host,
                        Send = _send,
                        Receive = _receive,
                        Lost = _lost,
                        RoundtripTime = pingResult.RoundtripTime
                    };

                    result = true;
                }
                else
                {
                    _lost++;
                }
            }
            return result;
        }

        private readonly string _host;
        private int _count;

        private int _send;
        private int _receive;
        private int _lost;
    }
}