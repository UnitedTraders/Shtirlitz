using System.IO;
using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Reporter.PingReporter
{
    public class PingReport : IReporter
    {
        public double Weight { get; }
        public string Name => "Ping/Trace reporter";

        public PingReport(IPingReporterProvider provider)
        {
            _provider = provider;
        }

        public void Report(string rootPath, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            while (_provider.Next())
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var message = $"Host {_provider.Current.Host} Send {_provider.Current.Send} Receive {_provider.Current.Receive} Lost {_provider.Current.Lost}\r\n";
                File.AppendAllText(rootPath, message);

                progressCallback?.Invoke(Weight);
            }
        }

        private readonly IPingReporterProvider _provider;
    }
}