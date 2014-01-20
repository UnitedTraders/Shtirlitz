using Shtirlitz.Reporter;
using System;
using System.Collections.Generic;
using System.IO;
using Shtirlitz.Reporter.TimeReporter;
using Xunit;

namespace Shtirlitz.Tests
{
    public class TimeReporterTests : ShtirlitzBaseTestClass
    {
        private static readonly DateTimeOffset CustomTime = new DateTime(2014, 1, 15, 0, 0, 0);

        private readonly TimeReporter reporter;

        public TimeReporterTests()
            : this(new TimeReporter(new TestTimeProvider(CustomTime)))
        {
        }

        public TimeReporterTests(TimeReporter reporter)
            : base(new List<IReporter> { reporter })
        {
            this.reporter = reporter;
        }

        [Fact]
        public void CreatesTheRightFile()
        {
            // act
            RunSynchronously(false);

            // assert
            Assert.True(File.Exists(Path.Combine(RootPath, reporter.GetFileNameFromTime(CustomTime))));
        }

        private class TestTimeProvider : ITimeProvider
        {
            private readonly DateTimeOffset time;

            public TestTimeProvider(DateTimeOffset time)
            {
                this.time = time;
            }

            public DateTimeOffset GetTime()
            {
                return time;
            }
        }
    }
}
