using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shtirlitz.Common;
using Shtirlitz.Reporter.TraceRouteReporter.Trace;

namespace Shtirlitz.Reporter.TraceRouteReporter
{
    public class MultiTracer : IMultiTracer
    {
        private readonly ITracer _tracer;
        private readonly ConcurrentDictionary<IPAddress, string> _hostNamesCash = new ConcurrentDictionary<IPAddress, string>();

        public MultiTracer(ITracer tracer)
        {
            _tracer = tracer;
        }

        #region Settings

        private int _maxHops = 30;
        public int MaxHops
        {
            get { return _maxHops; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                _maxHops = value;
            }
        }


        private int _pingTimeout = 1500;
        public int PingTimeout
        {
            get { return _pingTimeout; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                _pingTimeout = value;
            }
        }

        private int _pingTimeFrame = 1000;
        public int PingTimeFrame
        {
            get { return _pingTimeFrame; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                _pingTimeFrame = value;
            }
        }


        private int _packageSize = 64;
        public int PackageSize
        {
            get { return _packageSize; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                _packageSize = value;
            }
        }

        private int _numberOfRounds = 1;
        public int NumberOfRounds
        {
            get { return _numberOfRounds; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                _numberOfRounds = value;
            }
        }

        #endregion Settings

        public void Trace(IEnumerable<string> addresslist, Action<string> printResults, CancellationToken token, SimpleProgressCallback progressCallback = null)
        {

            var list = new List<string>(addresslist); //copy
            if (!list.Any())
                throw new Exception("Address list must contain at least one element");


            printResults(GenerateFormattedSettings());
            var pc = SimpleProgressCallback(progressCallback, list.Count);

            list.AsParallel().WithCancellation(token)
                .ForAll(addr => TraceAddress(addr, printResults, token, pc));
        }

        #region private

        private void TraceAddress(string addr, Action<string> printResults, CancellationToken token, SimpleProgressCallback progressCallback = null)
        {
            try
            {
                var traceRes = _tracer.Tracert(addr, MaxHops, PingTimeout, PackageSize).ToArray();
                //ToArray => In order to be able to display the detailed progress.
                var pc = SimpleProgressCallback(progressCallback, traceRes.Length);

                var results = traceRes
                    .AsParallel()
                    .AsOrdered()
                    .WithCancellation(token)
                    .Select(traceEntry => CalculateTraceEntry(traceEntry, token, pc)).ToArray();

                foreach (var routeEntry in
                        results.Where(entry => !entry.IsNull && entry.Address.ToString() == entry.HostName))
                {
                    string addresFromCashe;
                    if (_hostNamesCash.TryGetValue(routeEntry.Address, out addresFromCashe))
                    {
                        //If during the work were obtained hostNames => Then update the output info.
                        routeEntry.HostName = addresFromCashe;
                    }
                }

                var str = GenerateFormatedResult(addr, results);

                progressCallback.TryInvoke(1);
                printResults(str);
            }
            catch (PingException x)
            {
                var resstr = GenerateFormatedError(addr, x);
                printResults(resstr);
            }
            catch (AggregateException ae)
            {
                ae.Handle(x =>
                {
                    if (x is PingException)
                    {
                        var resstr = GenerateFormatedError(addr, x as PingException);
                        printResults(resstr);
                    }
                    return true;
                });
            }

        }

        private RouteEntry CalculateTraceEntry(TracertEntry te, CancellationToken token, SimpleProgressCallback progressCallback = null)
        {
            var re = new RouteEntry { Ttl = te.Ttl };
            if (te.Address == null)
            {
                re.Address = IPAddress.Any;
                re.HostName = "*";
                re.LastRoundTrip = 0;
                re.BestRoundTrip = 0;
                re.AvgRoundTrip = 0;
                re.WorstRoundTrip = 0;
                re.SentPings = 0;
                re.RecvPings = 0;
                re.Loss = 1;
                re.Text = "N/A";
                re.IsNull = true;
                return re;
            }

            string hostName;
            if (!_hostNamesCash.TryGetValue(te.Address, out hostName))
            {
                hostName = te.Address.ToString();
                _tracer.GetHostNameAsync(te.Address, UpdateCash);
            }
            re.Address = te.Address;
            re.HostName = hostName;
            re.LastRoundTrip = te.RoundTrip;
            re.BestRoundTrip = te.RoundTrip;
            re.AvgRoundTrip = te.RoundTrip;
            re.WorstRoundTrip = te.RoundTrip;
            re.SentPings = 1;
            re.RecvPings = 1;
            re.Loss = 0;
            re.Text = string.Empty;
            re.IsNull = false;


            var addressProgressMeasure = 1.0 / NumberOfRounds;
            var currentProggress = addressProgressMeasure;
            progressCallback.TryInvoke(currentProggress);

            for (var i = 1; i < NumberOfRounds; i++)//The first operation was called Ping tracing the route => 1
            {
                var pingres = _tracer.Ping(te.Address.ToString(), PingTimeout);
                UpdatePingResults(re, pingres);
                token.ThrowIfCancellationRequested();
                currentProggress += addressProgressMeasure;
                progressCallback.TryInvoke(currentProggress);
                Delay(PingTimeFrame, token).Wait(token);
            }

            progressCallback.TryInvoke(1);
            return re;
        }

        private void UpdateCash(IPAddress address, string s)
        {
            _hostNamesCash[address] = s;
        }

        private static void UpdatePingResults(RouteEntry routeEntry, PingEntry res)
        {
            routeEntry.SentPings++;
            routeEntry.AvgRoundTrip = (routeEntry.AvgRoundTrip + res.RoundTrip) / 2;
            routeEntry.LastRoundTrip = res.RoundTrip;
            if (res.ReplyStatus == IPStatus.Success)
            {
                routeEntry.BestRoundTrip = Math.Min(routeEntry.LastRoundTrip, routeEntry.BestRoundTrip);
                routeEntry.WorstRoundTrip = Math.Max(routeEntry.LastRoundTrip, routeEntry.WorstRoundTrip);
                routeEntry.RecvPings++;
            }
            if (routeEntry.SentPings < routeEntry.RecvPings)
            {
                //localhost?
                routeEntry.Loss = 0;
                routeEntry.RecvPings = routeEntry.SentPings;
            }
            else
            {
                routeEntry.Loss = ((float)(routeEntry.SentPings - routeEntry.RecvPings) / routeEntry.SentPings);
            }
        }

        private static SimpleProgressCallback SimpleProgressCallback(SimpleProgressCallback progressCallback, int itemscount)
        {
            double currentProggress = 0;
            var sl = new object();
            var addressProgressMeasure = 1.0 / itemscount;
            var pc = progressCallback == null
                ? null
                : new SimpleProgressCallback(
                    progress =>
                    {
                        lock (sl)
                        {
                            currentProggress += addressProgressMeasure * progress;
                            progressCallback.TryInvoke(currentProggress);
                        }
                    });
            return pc;
        }

        private static Task Delay(double milliseconds, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();
            token.Register(tcs.SetCanceled);
            var timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) => tcs.TrySetResult(true);
            timer.Interval = milliseconds;
            timer.AutoReset = false;
            timer.Start();
            return tcs.Task;
        }


        #region FormatOutput
        private string GenerateFormattedSettings()
        {
            var sbMain = new StringBuilder();
            sbMain.AppendLine("Trace Settings:");
            var props = typeof(MultiTracer).GetProperties();
            foreach (var propertyInfo in props)
            {
                var val = propertyInfo.GetValue(this, null);
                if (val is IEnumerable)
                {
                    var ie = val as IEnumerable;
                    var arr = (from object o in ie select o.ToString()).ToArray();
                    val = string.Join(";", arr);
                }

                sbMain.AppendFormat("{0}\t:{1};{2}", propertyInfo.Name, val, Environment.NewLine);
            }
            return sbMain.ToString();
        }

        private string GenerateFormatedResult(string addr, IEnumerable<RouteEntry> results)
        {
            var sbMain = new StringBuilder();
            sbMain.AppendLine(addr);
            var props = typeof(RouteEntry).GetProperties();
            foreach (var propertyInfo in props)
            {
                sbMain.AppendFormat("{0,10}\t", propertyInfo.Name);
            }
            sbMain.AppendLine();

            foreach (var routeEntry in results)
            {
                var sb = new StringBuilder(256);
                foreach (var prop in props)
                {
                    var value = prop.GetValue(routeEntry, null);
                    sb.AppendFormat("{0,10}\t", value);
                }
                sbMain.AppendFormat("{0}{1}", sb, Environment.NewLine);
            }
            return sbMain.ToString();
        }

        private string GenerateFormatedError(string addr, PingException exception)
        {
            var sbMain = new StringBuilder();
            sbMain.AppendLine(addr);
            sbMain.AppendLine("The specified address is not available or has the wrong format.");
            return sbMain.ToString();
        }
        #endregion FormatOutput

        #endregion private

    }

}
