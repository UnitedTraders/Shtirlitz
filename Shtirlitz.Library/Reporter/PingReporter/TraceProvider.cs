using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Shtirlitz.Reporter.PingReporter
{
    public class TraceProvider : IPingReporterProvider
    {
        public PingResult Current { get; private set; }

        public TraceProvider(string host, int jumps, int timeout, int count)
        {
            _host = host;
            _jumps = jumps;
            _timeout = timeout;
            _count = count;

            _finalIp = GetFinalIp();
            _stat = new Dictionary<IPAddress, PingResult>();
            _lost = new Dictionary<int, int>();
        }

        public bool Next()
        {
            var result = false;

            if (_finalIp != null
                && _count > 0)
            {
                var isfinal = SendPing();

                _currJumps++;
                if (_currJumps == _jumps
                    || isfinal)
                {
                    _currJumps = 0;
                    _count--;
                }

                result = true;
            }

            return result;
        }

        private readonly string _host;
        private readonly int _jumps;
        private readonly int _timeout;
        private readonly IPAddress _finalIp;
        private readonly Dictionary<IPAddress, PingResult> _stat;
        private readonly Dictionary<int, int> _lost;
        private int _count;

        private int _currJumps;

        private bool SendPing()
        {
            var result = false;

            ResetCurrent();
            using (var ping = new Ping())
            {
                var pingResult = ping.Send(_host, _timeout, new byte[0], new PingOptions() {Ttl = _currJumps + 1});
                if (pingResult?.Status == IPStatus.Success)
                {
                    var curr = UpdateStat(pingResult);
                    UpdateCurrent(curr);

                    result = Equals(pingResult.Address, _finalIp);
                }
                else
                {
                    UpdateLost();
                }
            }

            return result;
        }

        private PingResult UpdateStat(PingReply pingResult)
        {
            PingResult curr;
            if (!_stat.TryGetValue(pingResult.Address, out curr))
            {
                curr = new PingResult
                {
                    Host = pingResult.Address.ToString(),
                    Send = 1,
                    Receive = 1,
                    RoundtripTime = pingResult.RoundtripTime
                };

                _stat[pingResult.Address] = curr;
            }
            else
            {
                curr.Send++;
                curr.Receive++;
            }
            return curr;
        }

        private void UpdateLost()
        {
            int lost;
            if (_lost.TryGetValue(_currJumps, out lost))
                _lost[_currJumps] = lost + 1;
            else
                _lost[_currJumps] = 1;
        }

        private void ResetCurrent()
        {
            int lost;
            if (_lost.TryGetValue(_currJumps, out lost))
                Current = new PingResult { Host = _currJumps.ToString(), Send = lost + 1, Lost = lost + 1 };
            else
                Current = new PingResult { Host = _currJumps.ToString(), Send = 1, Lost = 1 };
        }

        private void UpdateCurrent(PingResult source)
        {
            Current = new PingResult
            {
                Host = source.Host,
                Send = source.Send,
                Receive = source.Receive,
                Lost = source.Lost,
                RoundtripTime = source.RoundtripTime
            };
        }

        private IPAddress GetFinalIp()
        {
            IPAddress result = null;

            using (var ping = new Ping())
            {
                var pingResult = ping.Send(_host, _timeout);
                if (pingResult?.Status == IPStatus.Success)
                    result = pingResult.Address;
            }
            return result;
        }
    }
}