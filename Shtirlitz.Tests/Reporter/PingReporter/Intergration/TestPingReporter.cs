using System.IO;
using System.Threading;
using Moq;
using NUnit.Framework;
using Shtirlitz.Reporter;
using Shtirlitz.Reporter.PingReporter;

namespace Shtirlitz.Tests.Reporter.PingReporter.Intergration
{
    [TestFixture]
    public class TestPingReporter : AssertionHelper
    {
        [Test]
        public void TestReport()
        {
            const string expected = "Host ::1 Send 1 Receive 1 Lost 0\r\n";

            var count = 1;
            var provider = new Mock<IPingReporterProvider>();
            provider.Setup(p => p.Next()).Returns(() => count-- > 0);
            provider.Setup(p => p.Current).Returns(() => new PingResult {Host = "::1", Send = 1, Receive = 1});

            var filename = Path.GetTempFileName();

            IReporter report = new PingReport(provider.Object);
            report.Report(filename, CancellationToken.None);

            var actual = File.ReadAllText(filename);

            Expect(actual, EqualTo(expected));
        }

        [Test]
        public void TestCancel()
        {
            var provider = new Mock<IPingReporterProvider>();
            provider.Setup(p => p.Next()).Returns(() => true);
            provider.Setup(p => p.Current).Returns(() => new PingResult { Host = "::1", Send = 1, Receive = 1 });

            var filename = Path.GetTempFileName();
            var stopSignal = new CancellationTokenSource();
            var stopThread = new Thread(new ThreadStart(() =>
            {
                IReporter report = new PingReport(provider.Object);
                report.Report(filename, stopSignal.Token);
            }));
            stopThread.IsBackground = true;
            stopThread.Start();

            stopSignal.Cancel();

            Expect(() => stopThread.IsAlive, False.After(1000));
        }

        [Test]
        public void TestCallback()
        {
            var count = 1;
            var provider = new Mock<IPingReporterProvider>();
            provider.Setup(p => p.Next()).Returns(() => count-- > 0);
            provider.Setup(p => p.Current).Returns(() => new PingResult { Host = "::1", Send = 1, Receive = 1 });

            var filename = Path.GetTempFileName();

            var actual = false;
            IReporter report = new PingReport(provider.Object);
            report.Report(filename, CancellationToken.None, progress => actual = true);

            Expect(actual, True);
        }
    }
}