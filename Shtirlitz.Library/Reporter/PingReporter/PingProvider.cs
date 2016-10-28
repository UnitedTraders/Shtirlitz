using System.Net.NetworkInformation;

namespace Shtirlitz.Reporter.PingReporter
{
    public class PingProvider : IPingReporterProvider
    {
        public PingResult Current { get; private set; }
        public double Weight
        {
            get
            {
                var result = 1;
                if (_count > 0)
                    result = 100 * _currCount / _count;
                return result;
            }
        }

        public PingProvider(string host, int count)
        {
            _host = host;
            _count = count;
            _currCount = count;
        }

        public bool Next()
        {
            var result = false;

            if (_currCount > 0)
            {
                result = SendPing();
                _currCount--;
            }

            return result;
        }

        private bool SendPing()
        {
            var result = false;

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
        private readonly int _count;
        private int _currCount;

        private int _send;
        private int _receive;
        private int _lost;
    }
}