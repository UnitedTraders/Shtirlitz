using NUnit.Framework;
using Shtirlitz.Reporter.PingReporter;
using Shtirlitz.Tests.Reporter.PingReporter.Common;

namespace Shtirlitz.Tests.Reporter.PingReporter.Intergration
{
    [TestFixture]
    public class TestPingProvider : AssertionHelper
    {
        [Test]
        public void TestPingSelf()
        {
            var expected = new PingResult
            {
                Host = "localhost",
                Send = 1,
                Receive = 1
            };

            IPingReporterProvider ping = new PingProvider("localhost", 2);
            ping.Next();
            var actual = ping.Current;

            Expect(actual, EqualTo(expected).Using(new PingResultCompareer()));
        }

        [Test]
        public void TestNext()
        {
            const bool expected = false;

            IPingReporterProvider ping = new PingProvider("localhost", 1);
            ping.Next();
            var actual = ping.Next();

            Expect(actual, EqualTo(expected));
        }
    }
}