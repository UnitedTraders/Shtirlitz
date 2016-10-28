using NUnit.Framework;
using Shtirlitz.Reporter.PingReporter;
using Shtirlitz.Tests.Reporter.PingReporter.Common;

namespace Shtirlitz.Tests.Reporter.PingReporter.Intergration
{
    [TestFixture]
    public class TestTraceProvider : AssertionHelper
    {
        [Test]
        public void TestTraceSelf()
        {
            var expected = new PingResult
            {
                Host = "::1",
                Send = 2,
                Receive = 2
            };

            IPingReporterProvider tracer = new TraceProvider("localhost", 1, 1000, 2);
            tracer.Next();
            tracer.Next();
            var actual = tracer.Current;

            Expect(actual, EqualTo(expected).Using(new PingResultCompareer()));
        }

        [Test]
        public void TestFinalTrace()
        {
            const bool expected = false;

            IPingReporterProvider tracer = new TraceProvider("localhost", 30, 1000, 1);
            tracer.Next();
            var actual = tracer.Next();

            Expect(actual, EqualTo(expected));
        }

        [Test]
        public void TestNext()
        {
            const bool expected = false;

            IPingReporterProvider ping = new TraceProvider("localhost", 1, 1000, 1);
            ping.Next();
            var actual = ping.Next();

            Expect(actual, EqualTo(expected));
        }
    }
}