using Shtirlitz.Reporter;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Shtirlitz.Tests
{
    public class TimeReporterTests : ShtirlitzBaseTestClass
    {
        private readonly TimeReporter _reporter;
        private static DateTimeOffset _customTime = new DateTime(2014, 1, 15, 0, 0, 0);

        #region Constructors
        public TimeReporterTests()
            : this(new TimeReporter(new TestTimeProvider(_customTime)))
        {
        }

        public TimeReporterTests(TimeReporter reporter)
            : base(new List<IReporter> { reporter })
        {
            _reporter = reporter;
        }
        #endregion

        [Fact]
        public void TimeReporter_ReportTest()
        {
            // act
            RunSynchronously(false);

            // assert
            Assert.True(File.Exists(Path.Combine(RootPath, _reporter.GetFileNameFromTime(_customTime))));
        }

        class TestTimeProvider : ITimeProvider
        {
            DateTimeOffset _time;

            public TestTimeProvider(DateTimeOffset time)
            {
                _time = time;
            }

            public DateTimeOffset GetTime()
            {
                return _time;
            }
        }
    }
}
