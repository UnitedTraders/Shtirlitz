using System.IO;
using System.Threading;
using NUnit.Framework;
using Shtirlitz.Reporter;
using Shtirlitz.Reporter.PingReporter;

namespace Shtirlitz.Tests.Reporter.PingReporter.Intergration
{
    [TestFixture]
    public class TestTraceToYaRu : AssertionHelper
    {
        [Test]
        public void TestTrace()
        {
            var filename = Path.GetTempFileName();

            var provider = new TraceProvider("ya.ru", 30, 1000, 2);

            IReporter report = new PingReport(provider);
            report.Report(filename, CancellationToken.None);

            var actual = File.ReadAllText(filename);

            Expect(actual, Not.Null);
        }
    }
}